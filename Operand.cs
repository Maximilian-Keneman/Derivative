using System;
using System.ComponentModel;
using System.Linq;

namespace Derivative
{
    internal enum Operation
    {
        Addition,
        Multiplication,
        Exponentiation,
        Logarithm,
        Subtraction,
        Division,
        NRoot,
        Sine,
        Cosine,
        Tangent
    }
    internal static class ExpansionOperationEnum
    {
        internal static Exception ThrowInvalidOperation(this Operation operation)
            => new InvalidEnumArgumentException(nameof(operation), (int)operation, operation.GetType());

        internal static string ToSymbol(this Operation operation) => operation switch
        {
            Operation.Addition => " + ",
            Operation.Subtraction => " - ",
            Operation.Multiplication => " * ",
            Operation.Division => " / ",
            Operation.Exponentiation => "^",
            Operation.Logarithm => "log",
            Operation.NRoot => "nrt",
            Operation.Sine => "sin",
            Operation.Cosine => "cos",
            Operation.Tangent => "tg",
            _ => throw operation.ThrowInvalidOperation(),
        };
        internal static bool TryParseToOperation(this string symbol, out Operation result)
        {
            Operation? operation = symbol switch
            {
                "sin" => Operation.Sine,
                "cos" => Operation.Cosine,
                "tg" => Operation.Tangent,
                "log" => Operation.Logarithm,
                "nrt" => Operation.NRoot,
                "^" => Operation.Exponentiation,
                "*" => Operation.Multiplication,
                "/" => Operation.Division,
                "+" => Operation.Addition,
                "-" => Operation.Subtraction,
                _ => null,
            };
            if (operation.HasValue)
            {
                result = operation.Value;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        internal static int GetPriority(this Operation operation) => operation switch
        {
            Operation.Sine => 0,
            Operation.Cosine => 0,
            Operation.Tangent => 0,
            Operation.Logarithm => 0,
            Operation.NRoot => 0,
            Operation.Exponentiation => 1,
            Operation.Multiplication => 2,
            Operation.Division => 2,
            Operation.Addition => 3,
            Operation.Subtraction => 3,
            _ => throw operation.ThrowInvalidOperation(),
        };
        internal static bool IsLongSymbol(this Operation operation) => operation switch
        {
            Operation.Sine => true,
            Operation.Cosine => true,
            Operation.Tangent => true,
            Operation.Logarithm => true,
            Operation.NRoot => true,
            Operation.Exponentiation => false,
            Operation.Multiplication => false,
            Operation.Division => false,
            Operation.Addition => false,
            Operation.Subtraction => false,
            _ => throw operation.ThrowInvalidOperation()
        };
    }

    internal abstract class Operand : ICloneable<Operand>
    {
        public static string ClearString(string function)
        {
            while (true)
                if (function[0] == '(' && function.Last() == ')')
                {
                    int n = 0;
                    bool b = true;
                    for (int i = 1; i < function.Length - 1; i++)
                    {
                        if (function[i] == '(')
                            n++;
                        else if (function[i] == ')')
                            n--;
                        if (n < 0)
                        {
                            b = false;
                            break;
                        }
                    }
                    if (b)
                        function = function.Substring(1, function.Length - 2);
                    else
                        break;
                }
                else
                    break;
            function = function.Replace(" ", "").Replace(".", ",");
            return function;
        }
        public static Operand CreateFunction(string function, char parameter)
        {
            function = ClearString(function);

            int CheckBracketsToI(int i)
            {
                int c = 0;
                for (int j = 0; j <= i; j++)
                    if (function[j] == '(')
                        c++;
                    else if (function[j] == ')')
                        c--;
                return c;
            }

            int[] BracketsPosition = new int[function.Length];
            bool BrClosed = false;
            for (int i = 0; i < function.Length; i++)
            {
                BracketsPosition[i] = CheckBracketsToI(i);
                if (BrClosed)
                    BrClosed = false;
                else if (i != 0 && BracketsPosition[i - 1] > BracketsPosition[i])
                {
                    BrClosed = true;
                    BracketsPosition[i]++;
                }
            }
            if (BracketsPosition.Last() != 0 && (BracketsPosition.Last() != 1 || function.Last() != ')'))
                throw new TextException("Not all parentheses are closed.");

            int LowPriorityOperationIndex = -1;
            int MIN = BracketsPosition.Min();
            int IND = BracketsPosition.ToList().LastIndexOf(MIN);
            Operation? oper = null;
            for (int i = IND; i >= 0; i--)
            {
                if (BracketsPosition[i] == MIN)
                {
                    if ("^*/+-".Contains(function[i]))
                    {
                        if (function[i].ToString().TryParseToOperation(out Operation checkOper) &&
                           (!oper.HasValue || checkOper.GetPriority() > oper.Value.GetPriority()))
                        {
                            oper = checkOper;
                            LowPriorityOperationIndex = i;
                        }
                    }
                    else if (i >= 2)
                    {
                        if (function.Substring(i - 2, 3).TryParseToOperation(out Operation checkOper))
                        {
                            i -= 2;
                            if (!oper.HasValue || checkOper.GetPriority() > oper.Value.GetPriority())
                            {
                                oper = checkOper;
                                LowPriorityOperationIndex = i;
                            }
                        }
                    }
                    else if (i >= 1)
                    {
                        if (function.Substring(i - 1, 2).TryParseToOperation(out Operation checkOper))
                        {
                            i--;
                            if (!oper.HasValue || checkOper.GetPriority() > oper.Value.GetPriority())
                            {
                                oper = checkOper;
                                LowPriorityOperationIndex = i;
                            }
                        }
                    }
                }
            }
            Operand GetSimpleOperand()
            {
                if (function == parameter.ToString())
                    return Parameter;
                else if (double.TryParse(function, out double number))
                    return number;
                else
                {
                    if (function.Length == 1 && char.IsLetter(function[0]))
                        throw new TextException("Invalid parameter's character.", function[0], 0);
                    else
                    {
                        int index = -1;
                        for (int i = 0; i < function.Length; i++)
                            if (function[i] != parameter && !char.IsDigit(function[i]))
                            {
                                index = i;
                                break;
                            }
                        throw new TextException("The string contains an invalid character.", function[index], index);
                    }
                }
            }
            Operand GetShortSymbolOperand(Operation operation)
            {
                string left = function.Substring(0, LowPriorityOperationIndex);
                string right = function.Substring(LowPriorityOperationIndex + 1);
                Operand rightOperand;
                if (string.IsNullOrEmpty(right))
                    throw new TextException("An empty operand.", LowPriorityOperationIndex);
                else
                    try
                    {
                        rightOperand = CreateFunction(right, parameter);
                    }
                    catch (TextException e)
                    {
                        throw new TextException(LowPriorityOperationIndex + 1, e);
                    }
                if (string.IsNullOrEmpty(left))
                {
                    if (operation == Operation.Subtraction)
                        return -rightOperand;
                    else
                        throw new TextException("An empty operand.", LowPriorityOperationIndex);
                }
                else
                {
                    Operand leftOperand;
                    try
                    {
                        leftOperand = CreateFunction(left, parameter);
                    }
                    catch (TextException e)
                    {
                        throw new TextException(0, e);
                    }
                    return new DoubleOperand(false, operation, leftOperand, rightOperand);
                }
            }
            Operand GetLongSymbolOperand(Operation operation)
            {
                if (operation == Operation.Logarithm || operation == Operation.NRoot)
                {
                    int INDSTR = LowPriorityOperationIndex + 3;
                    int VALUE = BracketsPosition[INDSTR];
                    int INDEND = BracketsPosition.ToList().LastIndexOf(VALUE);
                    int IND = -1;
                    for (int i = INDSTR; i <= INDEND; i++)
                        if (function[i] == ';' && BracketsPosition[i] == VALUE)
                        {
                            IND = i;
                            break;
                        }
                    if (IND > -1)
                    {
                        string left = function.Substring(INDSTR + 1, IND - INDSTR - 1);
                        string right = function.Substring(IND + 1, INDEND - IND - 1);
                        if (string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left))
                            throw new TextException("An empty operand.", IND);
                        else
                        {
                            Operand leftOperand, rightOperand;
                            try
                            {
                                leftOperand = CreateFunction(left, parameter);
                            }
                            catch (TextException e)
                            {
                                throw new TextException(INDSTR + 1, e);
                            }
                            try
                            {
                                rightOperand = CreateFunction(right, parameter);
                            }
                            catch (TextException e)
                            {
                                throw new TextException(IND + 1, e);
                            }
                            return new DoubleOperand(false, operation, leftOperand, rightOperand);
                        }
                    }
                    else
                        throw new TextException($"Couldn't find ';' in the {operation} operation box that separates the operands.", LowPriorityOperationIndex);
                }
                else if (operation == Operation.Sine || operation == Operation.Cosine || operation == Operation.Tangent)
                {
                    int length = operation.ToSymbol().Length;
                    int INDSTR = LowPriorityOperationIndex + length;
                    int INDEND = BracketsPosition.ToList().LastIndexOf(BracketsPosition[INDSTR]);
                    string operand = function.Substring(INDSTR + 1, INDEND - INDSTR - 1);
                    if (string.IsNullOrEmpty(operand))
                        throw new TextException("An empty operand.", INDSTR);
                    else
                    {
                        Operand Operand;
                        try
                        {
                            Operand = CreateFunction(operand, parameter);
                        }
                        catch (TextException e)
                        {
                            throw new TextException(INDSTR + 1, e);
                        }
                        return new SingleOperand(false, operation, Operand);
                    }
                }
                else
                    throw operation.ThrowInvalidOperation();
            }
            if (!oper.HasValue)
                return GetSimpleOperand();
            else if (oper.Value.IsLongSymbol())
                return GetLongSymbolOperand(oper.Value);
            else
                return GetShortSymbolOperand(oper.Value);
        }
        public abstract string ToString(char parameter);
        public override string ToString() => ToString('X');

        public Func<double, double> GetDelegate(int decimals) => X => Calculate(X, decimals);

        protected bool IsNegative { get; private set; }
        protected Operand(bool negative) => IsNegative = negative;

        public abstract bool IsConstant { get; }
        public abstract Operand Simplify(int decimals);
        public abstract Operand Substitute(Operand operand);
        public abstract double Calculate(double parameter, int decimals);
        public abstract Operand Derivative();
        public abstract Operand Clone();
        object ICloneable.Clone() => Clone();

        public static Operand Parameter => new SimpleOperand(false);
        public static implicit operator Operand(double number) => new SimpleOperand(number);

        public static Operand operator -(Operand operand)
        {
            operand.IsNegative = !operand.IsNegative;
            return operand;
        }
        public static Operand operator +(Operand left, Operand right) => right.IsNegative ?
                new DoubleOperand(false, Operation.Subtraction, left, -right) :
                new DoubleOperand(false, Operation.Addition, left, right);
        public static Operand operator -(Operand left, Operand right) => right.IsNegative ?
                new DoubleOperand(false, Operation.Addition, left, -right) :
                new DoubleOperand(false, Operation.Subtraction, left, right);
        public static Operand operator *(Operand left, Operand right)
        {
            bool isNegative = false;
            if (left.IsNegative)
            {
                isNegative = !isNegative;
                left = -left;
            }
            if (right.IsNegative)
            {
                isNegative = !isNegative;
                right = -right;
            }
            return new DoubleOperand(isNegative, Operation.Multiplication, left, right);
        }
        public static Operand operator *(bool isNegative, Operand operand)
        {
            if (isNegative)
                return -operand;
            else
                return operand;
        }
        public static Operand operator /(Operand left, Operand right)
        {
            bool isNegative = false;
            if (left.IsNegative)
            {
                isNegative = !isNegative;
                left = -left;
            }
            if (right.IsNegative)
            {
                isNegative = !isNegative;
                right = -right;
            }
            return new DoubleOperand(isNegative, Operation.Division, left, right);
        }

        public static bool operator ==(Operand operand, double @const) => operand.IsConstant && operand.Calculate(double.NaN, -1) == @const;
        public static bool operator !=(Operand operand, double @const) => !operand.IsConstant || operand.Calculate(double.NaN, -1) != @const;
        public static bool operator >(Operand operand, double @const) => operand.IsConstant && operand.Calculate(double.NaN, -1) > @const;
        public static bool operator <(Operand operand, double @const) => operand.IsConstant && operand.Calculate(double.NaN, -1) < @const;

        public static bool operator ==(Operand left, Operand right)
        {
            if (left is null || right is null)
                return false;
            if (left.IsConstant)
                return right == left.Calculate(double.NaN, -1);
            else if (right.IsConstant)
                return left == right.Calculate(double.NaN, -1);
            if (left.GetType() == right.GetType())
            {
                if (left is SimpleOperand)
                    return ((SimpleOperand)left).Equals((SimpleOperand)right);
                else if (left is DoubleOperand)
                    return ((DoubleOperand)left).Equals((DoubleOperand)right);
                else if (left is SingleOperand)
                    return ((SingleOperand)left).Equals((SingleOperand)right);
            }
            return false;
        }
        public static bool operator !=(Operand left, Operand right)
        {
            if (left is null || right is null)
                return false;
            if (left.IsConstant)
                return right != left.Calculate(double.NaN, -1);
            else if (right.IsConstant)
                return left != right.Calculate(double.NaN, -1);
            if (left.GetType() == right.GetType())
            {
                if (left is SimpleOperand)
                    return !((SimpleOperand)left).Equals((SimpleOperand)right);
                else if (left is DoubleOperand)
                    return !((DoubleOperand)left).Equals((DoubleOperand)right);
                else if (left is SingleOperand)
                    return !((SingleOperand)left).Equals((SingleOperand)right);
            }
            return true;
        }
    }

    internal class SingleOperand : Operand, IEquatable<SingleOperand>
    {
        private Operation Operation { get; }
        private Operand Operand { get; }

        public SingleOperand(bool negative, Operation operation, Operand operand) : base(negative)
        {
            if (operation != Operation.Sine &&
                operation != Operation.Cosine &&
                operation != Operation.Tangent)
                throw operation.ThrowInvalidOperation();
            Operation = operation;
            Operand = operand;
        }

        public override bool IsConstant => Operand.IsConstant;

        public override Operand Simplify(int decimals)
        {
            if (IsConstant)
                return Calculate(double.NaN, decimals);

            Operand operand = Operand.Simplify(decimals);

            switch (Operation)
            {
                case Operation.Sine:
                    break;
                case Operation.Cosine:
                    break;
                case Operation.Tangent:
                    break;
                default:
                    throw Operation.ThrowInvalidOperation();
            }
            return new SingleOperand(IsNegative, Operation, operand);
        }
        public override Operand Substitute(Operand operand) => new SingleOperand(IsNegative, Operation, Operand.Substitute(operand));
        public override double Calculate(double parameter, int decimals)
        {
            double result = (IsNegative ? -1 : 1) * Operation switch
            {
                Operation.Sine => Math.Sin(Operand.Calculate(parameter, decimals)),
                Operation.Cosine => Math.Cos(Operand.Calculate(parameter, decimals)),
                Operation.Tangent => Math.Tan(Operand.Calculate(parameter, decimals)),
                _ => throw Operation.ThrowInvalidOperation()
            };
            if (decimals == -1)
                return result;
            else if (decimals > -1)
                return Math.Round(result, decimals);
            else
                throw new ArgumentOutOfRangeException();
        }
        public override Operand Derivative()
        {
            if (IsConstant)
                return 0;
            else
                return Operation switch
                {
                    Operation.Sine => new SingleOperand(IsNegative, Operation.Cosine, Operand) * Operand.Derivative(),
                    Operation.Cosine => new SingleOperand(!IsNegative, Operation.Sine, Operand) * Operand.Derivative(),
                    Operation.Tangent => (IsNegative * (new SingleOperand(false, Operation.Sine, Operand) /
                                                        new SingleOperand(false, Operation.Cosine, Operand))).Derivative(),
                    _ => throw Operation.ThrowInvalidOperation()
                };
        }

        public override Operand Clone() => new SingleOperand(IsNegative, Operation, Operand.Clone());
        public override string ToString(char parameter)
        {
            string symbol = Operation.ToSymbol();

            if (Operand is null)
                throw new Exception("Внутренняя ошибка, обратитесь к разработчику\nOperand is null");
            string operand = Operand.ToString(parameter);

            return (IsNegative ? "-" : "") + $"{symbol}({operand})";
        }

        public bool Equals(SingleOperand other)
        {
            if (other is null)
                return false;
            return IsNegative == other.IsNegative &&
                   Operation == other.Operation &&
                   Operand == other.Operand;
        }
    }
    internal class DoubleOperand : Operand, IEquatable<DoubleOperand>
    {
        private Operation Operation { get; }
        private Operand LeftOperand { get; }
        private Operand RightOperand { get; }

        public DoubleOperand(bool negative, Operation operation, Operand leftOperand, Operand rightOperand) : base(negative)
        {
            if (operation == Operation.Sine ||
                operation == Operation.Cosine ||
                operation == Operation.Tangent)
                throw operation.ThrowInvalidOperation();
            Operation = operation;
            LeftOperand = leftOperand.IsConstant ? leftOperand.Calculate(double.NaN, -1) : leftOperand;
            RightOperand = rightOperand.IsConstant ? rightOperand.Calculate(double.NaN, -1) : rightOperand;
        }

        public override bool IsConstant => LeftOperand.IsConstant && RightOperand.IsConstant;
        public override Operand Simplify(int decimals)
        {
            if (IsConstant)
                return Calculate(double.NaN, decimals);

            Operand leftOperand = LeftOperand.Simplify(decimals);
            Operand rightOperand = RightOperand.Simplify(decimals);

            switch (Operation)
            {
                case Operation.Addition:
                case Operation.Subtraction:
                    if (leftOperand == 0)
                        return IsNegative * rightOperand;
                    else if (rightOperand == 0)
                        return IsNegative * leftOperand;
                    break;
                case Operation.Multiplication:
                    if (leftOperand == 0 || rightOperand == 0)
                        return 0;
                    else if (leftOperand == 1)
                        return IsNegative * rightOperand;
                    else if (rightOperand == 1)
                        return IsNegative * leftOperand;
                    else if (leftOperand == -1)
                        return IsNegative * -rightOperand;
                    else if (rightOperand == -1)
                        return IsNegative * -leftOperand;
                    break;
                case Operation.Division:
                    if (rightOperand == 0)
                        return double.NaN;
                    else if (leftOperand == 0)
                        return 0;
                    else if (leftOperand == rightOperand)
                        return 1;
                    else if (rightOperand == 1)
                        return IsNegative * leftOperand;
                    else if (rightOperand == -1)
                        return IsNegative * -leftOperand;
                    else if (leftOperand < 0)
                        return (!IsNegative * ((-leftOperand) / rightOperand)).Simplify(decimals);
                    else if (rightOperand < 0)
                        return (!IsNegative * (leftOperand / (-rightOperand))).Simplify(decimals);
                    break;
                case Operation.Exponentiation:
                    if (leftOperand == 0)
                    {
                        if (rightOperand == 0)
                            return 1;
                        else if (rightOperand > 0)
                            return 0;
                        else if (rightOperand < 0)
                            return double.NaN;
                    }
                    else if (leftOperand == 1)
                        return 1;
                    else if (rightOperand == 0)
                        return 0;
                    else if (rightOperand == 1)
                        return IsNegative * leftOperand;
                    else if (rightOperand == -1)
                        return (IsNegative * (1 / leftOperand)).Simplify(decimals);
                    break;
                case Operation.Logarithm:
                    if (leftOperand < 0)
                        return double.NaN;
                    else if (rightOperand < 0)
                        return double.NaN;
                    else if (rightOperand == 0)
                    {
                        if (leftOperand == 1)
                            return 0;
                        else
                            return double.NaN;
                    }
                    else if (rightOperand == 1)
                        return double.NaN;
                    else if (leftOperand == rightOperand)
                        return 1;
                    else if (leftOperand is DoubleOperand leftLogOperand && leftLogOperand.Operation == Operation.Exponentiation)
                        return (IsNegative *
                            (new DoubleOperand(false, Operation, leftLogOperand.LeftOperand, rightOperand) /
                            leftLogOperand.RightOperand)).Simplify(decimals);
                    else if (rightOperand is DoubleOperand rightLogOperand && rightLogOperand.Operation == Operation.Exponentiation)
                        return (IsNegative *
                            (rightLogOperand.RightOperand *
                            new DoubleOperand(false, Operation, leftOperand, rightLogOperand.LeftOperand))).Simplify(decimals);
                    break;
                case Operation.NRoot:
                    return new DoubleOperand(IsNegative, Operation.Exponentiation, leftOperand, 1 / rightOperand).Simplify(decimals);
                default:
                    throw Operation.ThrowInvalidOperation();
            }
            return new DoubleOperand(IsNegative, Operation, leftOperand, rightOperand);
        }
        public override Operand Substitute(Operand operand) => new DoubleOperand(IsNegative, Operation, LeftOperand.Substitute(operand), RightOperand.Substitute(operand));
        public override double Calculate(double parameter, int decimals)
        {
            double result = (IsNegative ? -1 : 1) * Operation switch
            {
                Operation.Addition => LeftOperand.Calculate(parameter, -1) + RightOperand.Calculate(parameter, -1),
                Operation.Multiplication => LeftOperand.Calculate(parameter, -1) * RightOperand.Calculate(parameter, -1),
                Operation.Exponentiation => Math.Pow(LeftOperand.Calculate(parameter, -1), RightOperand.Calculate(parameter, -1)),
                Operation.Logarithm => Math.Log(LeftOperand.Calculate(parameter, -1), RightOperand.Calculate(parameter, -1)),
                Operation.Subtraction => LeftOperand.Calculate(parameter, -1) - RightOperand.Calculate(parameter, -1),
                Operation.Division => LeftOperand.Calculate(parameter, -1) / RightOperand.Calculate(parameter, -1),
                Operation.NRoot => Math.Pow(LeftOperand.Calculate(parameter, -1), 1 / RightOperand.Calculate(parameter, -1)),
                _ => throw Operation.ThrowInvalidOperation()
            };
            if (decimals == -1)
                return result;
            else if (decimals > -1)
                return Math.Round(result, decimals);
            else
                throw new ArgumentOutOfRangeException();
        }
        public override Operand Derivative()
        {
            if (IsConstant)
                return 0;
            else
                return Operation switch
                {
                    Operation.Addition => IsNegative * (LeftOperand.Derivative() + RightOperand.Derivative()),
                    Operation.Subtraction => IsNegative * (LeftOperand.Derivative() - RightOperand.Derivative()),
                    Operation.Multiplication => IsNegative * ((LeftOperand.Derivative() * RightOperand) +
                                                              (LeftOperand * RightOperand.Derivative())),
                    Operation.Division => IsNegative * (LeftOperand * new DoubleOperand(false, Operation.Exponentiation, RightOperand, -1)).Derivative(),
                    Operation.Exponentiation => IsNegative * (this * (RightOperand * new DoubleOperand(false, Operation.Logarithm, LeftOperand, Math.E)).Derivative()),
                    Operation.Logarithm => RightOperand == Math.E ?
                        IsNegative * (LeftOperand.Derivative() / LeftOperand) :
                        IsNegative * (
                            new DoubleOperand(false, Operation.Logarithm, LeftOperand, Math.E) /
                            new DoubleOperand(false, Operation.Logarithm, RightOperand, Math.E)).Derivative(),
                    Operation.NRoot => new DoubleOperand(IsNegative, Operation.Exponentiation,
                        LeftOperand, new DoubleOperand(false, Operation.Exponentiation, RightOperand, -1)).Derivative(),
                    _ => throw Operation.ThrowInvalidOperation()
                };
        }

        public override Operand Clone() => new DoubleOperand(IsNegative, Operation, LeftOperand.Clone(), RightOperand.Clone());
        public override string ToString(char parameter)
        {
            string symbol = Operation.ToSymbol();

            if (LeftOperand is null)
                throw new Exception("Внутренняя ошибка, обратитесь к разработчику\nLeftOperand is null");
            string left = LeftOperand.ToString(parameter);
            if (LeftOperand is DoubleOperand leftOperand &&
                !Operation.IsLongSymbol() && !leftOperand.Operation.IsLongSymbol() &&
                leftOperand.Operation.GetPriority() > Operation.GetPriority())
                left = $"({LeftOperand.ToString(parameter)})";

            if (RightOperand is null)
                throw new Exception("Внутренняя ошибка, обратитесь к разработчику\nRightOperand is null");
            string right = RightOperand.ToString(parameter);;
            if (RightOperand is DoubleOperand rightOperand &&
                !Operation.IsLongSymbol() && !rightOperand.Operation.IsLongSymbol() &&
                rightOperand.Operation.GetPriority() > Operation.GetPriority())
                right = $"({RightOperand.ToString(parameter)})";

            return (Operation.IsLongSymbol(), IsNegative) switch
            {
                (false, false) => string.Concat(left, symbol, right),
                (true, false) => $"{symbol}({left}; {right})",
                (false, true) => $"-({left}{symbol}{right})",
                (true, true) => $"-{symbol}({left}; {right})",
            };
        }

        public bool Equals(DoubleOperand other)
        {
            if (other is null)
                return false;
            return IsNegative == other.IsNegative &&
                   Operation == other.Operation &&
                   LeftOperand == other.LeftOperand &&
                   RightOperand == other.RightOperand;
        }
    }
    internal class SimpleOperand : Operand, IEquatable<SimpleOperand>
    {
        private double? Number { get; }

        public SimpleOperand(bool negative) : base(negative) => Number = null;
        public SimpleOperand(double number) : base(number < 0) => Number = Math.Abs(number);

        public override bool IsConstant => Number.HasValue;
        public override Operand Simplify(int decimals)
        {
            if (Number.HasValue && decimals > -1)
            {
                double number = Math.Round(Number.Value, decimals);
                if (number == Math.Round(Math.E, decimals))
                    return IsNegative * (SingleOperand)Math.E;
                else if (number == Math.Round(Math.PI, decimals))
                    return IsNegative * (SingleOperand)Math.PI;
                else if (number == 1d)
                    return IsNegative * (SingleOperand)1;
                else if (number == 0d)
                    return 0;
            }
            return this;
        }
        public override Operand Substitute(Operand operand) => Number.HasValue ? Clone() : operand.Clone();
        public override double Calculate(double parameter, int decimals)
        {
            double result = (IsNegative ? -1 : 1) * (Number ?? parameter);
            if (decimals == -1)
                return result;
            else if (decimals > -1)
                return Math.Round(result, decimals);
            else
                throw new ArgumentOutOfRangeException();
        }
        public override Operand Derivative() => Number.HasValue ? 0 : (IsNegative ? -1 : 1);
        public override Operand Clone() => Number.HasValue ? (IsNegative ? -1 : 1) * Number : IsNegative * Parameter;

        public override string ToString(char parameter) => (IsNegative ? "-" : "") + (Number?.ToString() ?? parameter.ToString());

        public bool Equals(SimpleOperand other)
        {
            if (other is null)
                return false;
            if (IsNegative == other.IsNegative)
                if (IsConstant && other.IsConstant)
                    return Number == other.Number;
                else
                    return !IsConstant && !other.IsConstant;
            return false;
        }
    }
}
