using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FreedomEngine.Content.Pipeline.Cutscenes
{
    [ContentProcessor(DisplayName = "Cutscene Processor")]
    public class CutsceneProcessor : ContentProcessor<List<string>, CutsceneContent>
    {
        private class BlockContext
        {
            public string Type;
            public int StartIndex;
            public int JumpInstructionIndex;
            public string LoopVarName;
        }

        // Memory mapping
        private readonly Dictionary<string, byte> _variables = [];
        private readonly Dictionary<int, byte> _literalRegisters = [];
        private readonly Dictionary<string, int> _labels = [];
        private readonly List<(int InstructionIndex, string LabelName)> _unresolvedJumps = [];

        private byte _nextVarIndex = 0;

        public override CutsceneContent Process(List<string> input, ContentProcessorContext context)
        {
            var content = new CutsceneContent();
            var initBlock = new List<Instruction>(); // Stores literal assignments (registers)
            var bodyInstructions = new List<Instruction>(); // Stores the actual program
            var stack = new Stack<BlockContext>();

            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || line.StartsWith(';'))
                    continue;

                string[] parts = SplitLine(line);
                string command = parts[0].ToLower();

                switch (command)
                {
                    case "@text":
                        ProcessTextCommand(parts, content, bodyInstructions);
                        break;
                    case "@var":
                        ProcessVarCommand(parts, bodyInstructions);
                        break;
                    case "@add":
                        ProcessAddCommand(parts, initBlock, bodyInstructions);
                        break;
                    case "@if":
                        ProcessIfCommand(line, initBlock, bodyInstructions, stack);
                        break;
                    case "@else":
                        ProcessElseCommand(bodyInstructions, stack);
                        break;
                    case "@endif":
                        ProcessEndIfCommand(bodyInstructions, stack);
                        break;
                    case "@label":
                        if (parts.Length >= 2) _labels[parts[1]] = bodyInstructions.Count;
                        break;
                    case "@jumplabel":
                        if (parts.Length >= 2)
                        {
                            int jumpIdx = bodyInstructions.Count;
                            bodyInstructions.Add(new Instruction { OpCode = OpCode.Jump, Parameters = [-1] });
                            _unresolvedJumps.Add((jumpIdx, parts[1]));
                        }
                        break;
                    case "@for":
                        ProcessForCommand(line, initBlock, bodyInstructions, stack);
                        break;
                    case "@endfor":
                        ProcessEndForCommand(initBlock, bodyInstructions, stack);
                        break;
                }
            }

            // Safely terminate the program
            bodyInstructions.Add(new Instruction { OpCode = OpCode.EndCutscene, Parameters = [] });

            // Pass 2: Resolve forward/backward label jumps
            foreach (var pending in _unresolvedJumps)
            {
                if (_labels.TryGetValue(pending.LabelName, out int targetIdx))
                {
                    bodyInstructions[pending.InstructionIndex].Parameters[0] = targetIdx;
                }
            }

            // Merge InitBlock and Body, offsetting all jump indices accordingly
            int jumpOffset = initBlock.Count;
            content.Instructions.AddRange(initBlock);

            foreach (var inst in bodyInstructions)
            {
                if (inst.OpCode == OpCode.Jump || inst.OpCode == OpCode.JumpIfNot)
                {
                    if (inst.Parameters != null && inst.Parameters.Count > 0 && inst.Parameters[0] >= 0)
                    {
                        inst.Parameters[0] += jumpOffset;
                    }
                }
                content.Instructions.Add(inst);
            }
            return content;
        }

        // --- Memory Allocation ---

        private byte GetOrAddVariable(string name)
        {
            if (!_variables.TryGetValue(name, out byte index))
            {
                index = _nextVarIndex++;
                _variables[name] = index;
            }
            return index;
        }

        private byte GetLiteralRegister(int value, List<Instruction> initBlock)
        {
            // Transforms a literal number into a managed variable register populated at start
            if (!_literalRegisters.TryGetValue(value, out byte index))
            {
                index = GetOrAddVariable($"__lit_{value}");
                _literalRegisters[value] = index;

                initBlock.Add(new Instruction
                {
                    OpCode = OpCode.SetVar,
                    Parameters = [index, value]
                });
            }
            return index;
        }

        // --- Command Processors ---

        private void ProcessTextCommand(string[] parts, CutsceneContent content, List<Instruction> body)
        {
            if (parts.Length < 2) return;

            string textArg = parts[1];
            if (textArg.StartsWith('\"') && textArg.EndsWith('\"'))
                textArg = textArg[1..^2]; //textArg.Substring(1, textArg.Length - 2);

            body.Add(new Instruction
            {
                OpCode = OpCode.PrintText,
                Parameters = [GetOrAddString(content.StringTable, textArg)]
            });
        }

        private void ProcessVarCommand(string[] parts, List<Instruction> body)
        {
            // @var assigns a literal value straight to the target variable, so it can
            // emit a single SetVar in place instead of going through an auxiliary
            // __lit_x register plus a CopyVar (that indirection is only needed when a
            // literal has to be compared against a variable, e.g. inside @if).
            if (parts.Length >= 3 && int.TryParse(parts[2], out int val))
            {
                byte varIdx = GetOrAddVariable(parts[1]);
                body.Add(new Instruction { OpCode = OpCode.SetVar, Parameters = [varIdx, val] });
            }
        }

        private void ProcessAddCommand(string[] parts, List<Instruction> initBlock, List<Instruction> body)
        {
            if (parts.Length >= 3 && int.TryParse(parts[2], out int val))
            {
                byte varIdx = GetOrAddVariable(parts[1]);
                byte valIdx = GetLiteralRegister(val, initBlock);
                body.Add(new Instruction { OpCode = OpCode.Add, Parameters = [varIdx, valIdx] });
            }
        }

        private void ProcessIfCommand(string line, List<Instruction> initBlock, List<Instruction> body, Stack<BlockContext> stack)
        {
            var match = Regex.Match(line, @"@if\s*\(\s*([a-zA-Z0-9_]+)\s*([><=]+)\s*([0-9]+)\s*\)");
            if (match.Success)
            {
                byte varIdx = GetOrAddVariable(match.Groups[1].Value);
                byte valIdx = GetLiteralRegister(int.Parse(match.Groups[3].Value), initBlock);

                switch (match.Groups[2].Value)
                {
                    case ">":
                        body.Add(new Instruction { OpCode = OpCode.CheckVar, Parameters = [varIdx, 0, valIdx] });
                        break;
                    case "<":
                        body.Add(new Instruction { OpCode = OpCode.CheckVar, Parameters = [varIdx, 1, valIdx] });
                        break;
                    case "==":
                        body.Add(new Instruction { OpCode = OpCode.CheckVar, Parameters = [varIdx, 2, valIdx] });
                        break;
                    case "!=":
                        body.Add(new Instruction { OpCode = OpCode.CheckVar, Parameters = [varIdx, 3, valIdx] });
                        break;
                    case "<=":
                        body.Add(new Instruction { OpCode = OpCode.CheckVar, Parameters = [varIdx, 4, valIdx] });
                        break;
                    case ">=":
                        body.Add(new Instruction { OpCode = OpCode.CheckVar, Parameters = [varIdx, 5, valIdx] });
                        break;
                }

                int jumpIdx = body.Count;
                body.Add(new Instruction { OpCode = OpCode.JumpIfNot, Parameters = [-1] });

                stack.Push(new BlockContext { Type = "if", JumpInstructionIndex = jumpIdx });
            }
        }

        private void ProcessElseCommand(List<Instruction> body, Stack<BlockContext> stack)
        {
            if (stack.Count > 0 && stack.Peek().Type == "if")
            {
                var ifContext = stack.Pop();

                int jumpIdx = body.Count;
                body.Add(new Instruction { OpCode = OpCode.Jump, Parameters = [-1] });
                body[ifContext.JumpInstructionIndex].Parameters[0] = body.Count;
                stack.Push(new BlockContext { Type = "else", JumpInstructionIndex = jumpIdx });
            }
        }

        private void ProcessEndIfCommand(List<Instruction> body, Stack<BlockContext> stack)
        {
            if (stack.Count > 0 && (stack.Peek().Type == "if" || stack.Peek().Type == "else"))
            {
                var context = stack.Pop();
                body[context.JumpInstructionIndex].Parameters[0] = body.Count;
            }
        }

        private void ProcessForCommand(string line, List<Instruction> initBlock, List<Instruction> body, Stack<BlockContext> stack)
        {
            var match = Regex.Match(line, @"@for\s*\(\s*([a-zA-Z0-9_]+)\s+([0-9]+)\s*\)");
            if (match.Success)
            {
                string iterVar = match.Groups[1].Value;
                byte iterIdx = GetOrAddVariable(iterVar);
                byte maxIdx = GetLiteralRegister(int.Parse(match.Groups[2].Value), initBlock);

                // Init: i = 0 (same case as @var — a direct literal assignment,
                // no need for a shared __lit_0 register + CopyVar here)
                body.Add(new Instruction { OpCode = OpCode.SetVar, Parameters = [iterIdx, 0] });

                int startIndex = body.Count;

                // Condition: i < max
                body.Add(new Instruction { OpCode = OpCode.CheckVar, Parameters = [iterIdx, 1, maxIdx] });

                int jumpIdx = body.Count;
                body.Add(new Instruction { OpCode = OpCode.JumpIfNot, Parameters = [-1] });

                stack.Push(new BlockContext { Type = "for", StartIndex = startIndex, JumpInstructionIndex = jumpIdx, LoopVarName = iterVar });
            }
        }

        private void ProcessEndForCommand(List<Instruction> initBlock, List<Instruction> body, Stack<BlockContext> stack)
        {
            if (stack.Count > 0 && stack.Peek().Type == "for")
            {
                var context = stack.Pop();
                byte iterIdx = GetOrAddVariable(context.LoopVarName);
                byte oneIdx = GetLiteralRegister(1, initBlock);

                // Increment: i += 1
                body.Add(new Instruction { OpCode = OpCode.Add, Parameters = [iterIdx, oneIdx] });

                // Loop back to condition
                body.Add(new Instruction { OpCode = OpCode.Jump, Parameters = [context.StartIndex] });

                // Resolve exit jump
                body[context.JumpInstructionIndex].Parameters[0] = body.Count;
            }
        }

        // --- Utilities ---

        private static string[] SplitLine(string line)
        {
            var matches = Regex.Matches(line, @"[\""].+?[\""]|[^ ]+");
            var result = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++) result[i] = matches[i].Value;
            return result;
        }

        private static int GetOrAddString(List<string> table, string value)
        {
            int index = table.IndexOf(value);
            if (index == -1)
            {
                index = table.Count;
                table.Add(value);
            }
            return index;
        }
    }
}