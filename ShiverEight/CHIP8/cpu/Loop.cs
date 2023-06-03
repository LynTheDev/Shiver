namespace ShiverEight.CHIP8.cpu;

public class Loop
{
    public bool Running = true;

    private readonly State CpuState;

    public Loop(State state)
    {
        CpuState = state;
    }

    public void Execute()
    {
        while (Running)
        {
            // Fetch
            ushort instruction = (ushort) (CpuState.Memory[CpuState.PC] << 8 | CpuState.Memory[CpuState.PC + 1]);
            // Increment by 2 bytes
            CpuState.PC += 2;

            ushort X = (ushort)((instruction & 0x0F00) >> 8); // shifting 8 to shave those 0s
            ushort Y = (ushort)((instruction & 0x00F0) >> 4); // same here
            ushort N = (ushort)(instruction & 0x000F);        // we dont need to do that here bc deez nu-
            ushort NN = (ushort)(instruction & 0x00FF);
            ushort NNN = (ushort)(instruction & 0x0FFF);

            // Decode
            switch (instruction & 0xF000)
            {
                case 0x0000:
                    if (instruction == 0x00E0)
                    {
                        Array.Clear(CpuState.Display, 0, CpuState.Display.Length);
                    }
                    else if (instruction == 0x00EE)
                    {
                        CpuState.PC = CpuState.ChipStack.Pop();
                    }

                    break;
                case 0x1000:
                    CpuState.PC = NNN;
                    break;
                case 0x2000:
                    CpuState.ChipStack.Push(CpuState.PC);
                    CpuState.PC = NNN;
                    break;
                case 0x3000:
                    if (CpuState.RegV[X] == NN)
                        CpuState.PC += 2;
                    break;
                case 0x4000:
                    if (CpuState.RegV[X] != NN)
                        CpuState.PC += 2;
                    break;
                case 0x5000:
                    if (CpuState.RegV[X] == CpuState.RegV[Y])
                        CpuState.PC += 2;
                    break;
                case 0x6000:
                    CpuState.RegV[X] = NN;
                    break;
                case 0x7000:
                    CpuState.RegV[X] += NN;
                    break;
                case 0x8000:
                    switch (instruction & 0x000F)
                    {
                        case 0x0:
                            CpuState.RegV[X] = CpuState.RegV[Y];
                            break;
                        case 0x1:
                            CpuState.RegV[X] = (ushort)(CpuState.RegV[X] | CpuState.RegV[Y]);
                            break;
                        case 0x2:
                            CpuState.RegV[X] = (ushort)(CpuState.RegV[X] & CpuState.RegV[Y]);
                            break;
                        case 0x3:
                            CpuState.RegV[X] = (ushort)(CpuState.RegV[X] ^ CpuState.RegV[Y]);
                            break;
                        case 0x4:
                            int sum = CpuState.RegV[X] += CpuState.RegV[Y];

                            CpuState.RegV[0xF] = 0;

                            if (sum > 0xFF)
                                CpuState.RegV[0xF] = 1;

                            CpuState.RegV[X] = (byte)sum;

                            break;
                        case 0x5:
                            CpuState.RegV[0xF] = 0;
                            
                            if (CpuState.RegV[X] > CpuState.RegV[Y])
                                CpuState.RegV[0xF] = 1;

                            CpuState.RegV[X] = (ushort)(CpuState.RegV[X] - CpuState.RegV[Y]);
                            break;
                        case 0x6:
                            break;
                        case 0x7:
                            CpuState.RegV[0xF] = 0;

                            if (CpuState.RegV[Y] > CpuState.RegV[X])
                                CpuState.RegV[0xF] = 1;

                            CpuState.RegV[X] = (ushort)(CpuState.RegV[Y] - CpuState.RegV[X]);
                            break;
                        case 0xE:
                            break;
                    }

                    break;
                case 0x9000:
                    break;
                case 0xa000:
                    CpuState.RegI = NNN;
                    break;
                case 0xb000:
                    break;
                case 0xc000:
                    CpuState.RegV[X] = (ushort)(Random.Shared.Next(0, 0xFF + 1) & NN);
                    break;
                case 0xd000:
                    X = (ushort)(CpuState.RegV[X] % 64);
                    Y = (ushort)(CpuState.RegV[Y] % 32);

                    CpuState.RegV[0xf] = 0;

                    for (ushort rows = 0; rows < N; rows++)
                    {
                        if (Y + rows >= 32)
                        {
                            break;
                        }

                        var sprite = CpuState.Memory[CpuState.RegI + rows];
                        
                        for (ushort cols = 0; cols < 8; cols++)
                        {
                            if (X + cols >= 64)
                            {
                                break;
                            }

                            if ((sprite & (1 << (7 - cols))) != 0)
                            {
                                if (CpuState.Display[X + cols, Y + rows])
                                {
                                    CpuState.RegV[0xf] = 1;
                                }

                                CpuState.Display[X + cols, Y + rows] = !CpuState.Display[X + cols, Y + rows];
                            }
                        }
                    }
                        
                    break;
                case 0xe000:
                    break;
                case 0xf000:
                    switch (instruction & 0x00FF)
                    {
                        case 0x07:
                            CpuState.RegV[X] = CpuState.TimerD;
                            break;
                        case 0x15:
                            CpuState.TimerD = (byte)CpuState.RegV[X];
                            break;
                        case 0x18:
                            CpuState.SoundT = (byte)CpuState.RegV[X];
                            break;
                        case 0x0A:
                            break;
                        case 0x1E:
                            CpuState.RegI += (byte)CpuState.RegV[X];
                            break;
                        case 0x29:
                            CpuState.RegI = (ushort)(State.FontsAddress + (CpuState.RegV[X] & 0x0f) * 4);
                            break;
                    }

                    break;
                default:
                    Console.Error.WriteLine($"Unknown Instruction {instruction}");
                    break;
            }
        }
    }
}
