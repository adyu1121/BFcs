namespace BrainFuck
{
    enum error
    {
        LoopisNotEnd = 1, LoopisNotStart = 2,
        indexError = 3,indexMax = 4,
        overflow = 5,underflow = 6,
        CodeEnd = 7
    }
    class bf
    {
        enum symbolList
        {
            Plus = '+',
            Minus = '-',
            PointUp = '>',
            PointDown = '<',
            Input = ',',
            OutPut = '.',
            LoopStart = '[',
            LoopEnd = ']'
        }
        public delegate char Input();
        public delegate void Output(char ASCII);

        public string Code
        {
            get
            {
                string result = string.Empty;
                foreach (symbolList symbol in BFCode)
                {
                    result += (char)symbol;
                }
                return result;
            }
            set
            {
                BFCode.Clear();
                init();
                foreach (char ch in value)
                {
                    BFCode.Add((symbolList)ch);
                }
            }
        }
        public long this[int i]
        {
            get { return Memory[i]; }
            set { Memory[i] = value; }
        }

        public int index { get; private set; }
        public int Pointer { get; private set; }
        private long[] Memory = new long[32768];
        private Stack<int> LoopStack = new Stack<int>();


        private List<symbolList> BFCode = new List<symbolList>();
        private Input input;
        private Output output;

        public bf(Input input, Output output)
        {
            init();
            Code = string.Empty;
            this.input = input;
            this.output = output;
        }
        public bf(string code, Input input, Output output)
        {
            init();
            Code = code;
            this.input = input;
            this.output = output;
        }


        public void init()
        {
            index = 0;
            Pointer = 0;
            Array.Fill(Memory,0);
            LoopStack.Clear();
        }
        public bool Run()
        {
            int _Point = 0;
            long[] _Memory = new long[32768];
            Stack<int> _LoopStack = new Stack<int>();
            for (int _index = 0; _index < BFCode.Count; _index++)
            {
                switch (BFCode[_index])
                {
                    case symbolList.Plus:
                        if (Memory[Pointer] == long.MaxValue) return false;
                        _Memory[_Point]++;
                        break;
                    case symbolList.Minus:
                        if (Memory[Pointer] == long.MinValue) return false;
                        _Memory[_Point]--;
                        break;
                    case symbolList.PointUp:
                        if (_Point == 32767) return false;
                        _Point++;
                        break;
                    case symbolList.PointDown:
                        if (_Point == 0) return false;
                        _Point--;
                        break;
                    case symbolList.Input:
                        _Memory[_Point] = input();
                        break;
                    case symbolList.OutPut:
                        output((char)_Memory[_Point]);
                        break;
                    case symbolList.LoopStart:
                        if (_Memory[_Point] != 0)
                        {
                            _LoopStack.Push(_index);
                        }
                        else
                        {
                            int NestedLoopCount = 0;
                            while (true)
                            {
                                _index++;
                                if (_index > BFCode.Count - 1) return false;
                                if (BFCode[_index] == symbolList.LoopStart)
                                {
                                    NestedLoopCount++;
                                }
                                if (BFCode[_index] == symbolList.LoopEnd)
                                {

                                    if (NestedLoopCount != 0)
                                    {
                                        NestedLoopCount--;
                                    }
                                     else break;
                                }
                            }
                        }
                        break;
                    case symbolList.LoopEnd:
                        if (_LoopStack.Count == 0) return false;
                        _index = _LoopStack.Pop() - 1;
                        break;
                }
            }
            if(_LoopStack.Count != 0) return false;
            return true;
        }
        public bool Stap(out error errorCode)
        {
            if (index > BFCode.Count - 1)
            {
                if (LoopStack.Count != 0)
                {
                    errorCode = error.LoopisNotEnd;
                    return false;
                }
                else
                {
                    errorCode = error.CodeEnd;
                    return false;
                }

            }

            switch (BFCode[index])
            {
                case symbolList.Plus:
                    if (Memory[Pointer] == long.MaxValue)
                    {
                        errorCode = error.overflow;
                        return false;
                    }
                    Memory[Pointer]++;
                    break;
                case symbolList.Minus:
                    if (Memory[Pointer] == long.MinValue)
                    {
                        errorCode = error.underflow;
                        return false;
                    }
                    Memory[Pointer]--;
                    break;
                case symbolList.PointUp:
                    if (Pointer == 32767)
                    {
                        errorCode = error.indexMax;
                        return false;
                    }
                    Pointer++;
                    break;
                case symbolList.PointDown:
                    if (Pointer == 0)
                    {
                        errorCode = error.indexError;
                        return false;
                    }
                    Pointer--;
                    break;
                case symbolList.Input:
                    Memory[Pointer] = input();
                    break;
                case symbolList.OutPut:
                    output((char)Memory[Pointer]);
                    break;
                case symbolList.LoopStart:
                    if (Memory[Pointer] != 0)
                    {
                        LoopStack.Push(index);
                    }
                    else
                    {
                        int NestedLoopCount = 0;
                        while (true)
                        {
                            index++;
                            if (index > BFCode.Count - 1)
                            {
                                errorCode = error.LoopisNotEnd;
                                return false;
                            }
                            if (BFCode[index] == symbolList.LoopStart)
                            {
                                NestedLoopCount++;
                            }
                            if (BFCode[index] == symbolList.LoopEnd)
                            {
                                if (NestedLoopCount != 0)
                                {
                                    NestedLoopCount--;
                                }
                                else break;
                            }
                        }
                    }
                    break;
                case symbolList.LoopEnd:
                    if(LoopStack.Count == 0) {
                        errorCode = error.LoopisNotStart;
                        return false;
                    }
                    index = LoopStack.Pop() - 1;
                    break;
            }
            index++;
            errorCode = 0;
            return true;
        }
    }
}
