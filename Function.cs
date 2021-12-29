using System;

namespace Derivative
{
    /// <summary>
    /// Класс для содержания функции
    /// </summary>
    public class Function
    {
        private Operand Func;
        private char ParameterChar;
        private int Decimals;

        /// <summary>
        /// Заменяет букву, обозначающую аргумент функции.
        /// </summary>
        /// <param name="value">Новая буква для обозначения аргумента функции</param>
        /// <returns>Если <paramref name="value"/> не является буквой, <see cref="ParameterChar"/> не изменяется и возвращается <see langword="false"/>, иначе принимается новое значение и возвращается <see langword="true"/></returns>
        public bool SetParameterChar(char value)
        {
            if (char.IsLetter(value))
            {
                ParameterChar = value.ToString().ToLower()[0];
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Изменяет порядок округления
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Если <paramref name="value"/> меньше -1, <see cref="Decimals"/> не изменяется и возвращается <see langword="false"/>, иначе принимается новое значение и возвращается <see langword="true"/></returns>
        public bool SetDecimals(int value)
        {
            if (value < -1)
                return false;
            else
            {
                Decimals = value;
                return true;
            }
        }

        /// <summary>
        /// Создаёт экземпляр <see cref="Function"/> из строки необходимого стандарта. Нарушение стандарта вызовет исключение <see cref="TextException"/>.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>
        ///         Все скобки должны быть закрыты
        ///     </item>
        ///     <item>
        ///         Аргумент должен быть обозначен <paramref name="argument"/> с учётом регистра
        ///     </item>
        ///     <item>
        ///         Подряд стоящие знаки действий не допускаются. Исключение: +-
        ///     </item>
        ///     <item>
        ///         Опускание знака умножения не допускается
        ///     </item>
        ///     <item>
        ///         Операнд/операнды для действий, задающихся ключевыми словами, должны быть заключены в скобки и при необходимости разделятся ';'
        ///         <list type="table">
        ///             <listheader>Ключевые слова</listheader>
        ///             <item>
        ///                 <term>Логарифм</term>
        ///                 <description><c>log(логарифмируемое выражение;основание логарифма)</c></description>
        ///             </item>
        ///             <item>
        ///                 <term>Корень N-ой степени</term>
        ///                 <description><c>nrt(подкоренное выражение;степень корня)</c></description>
        ///             </item>
        ///             <item>
        ///                 <term>Синус</term>
        ///                 <description><c>sin(угол в радианах)</c></description>
        ///             </item>
        ///             <item>
        ///                 <term>Косинус</term>
        ///                 <description><c>cos(угол в радианах)</c></description>
        ///             </item>
        ///             <item>
        ///                 <term>Тангенс</term>
        ///                 <description><c>tg(угол в радианах)</c></description>
        ///             </item>
        ///         </list>
        ///     </item>
        /// </list>
        /// </remarks>
        /// <param name="text">Строка для преобразования в функцию</param>
        /// <param name="argument">Буква для обозначения аргумента функции</param>
        /// <param name="decimals">Порядок округления - число знаков в десятичной части</param>
        /// <exception cref="TextException"></exception>
        public Function(string text, char argument = 'x', int decimals = -1) :
            this(Operand.CreateFunction(text, argument), argument, decimals)
        { }
        private Function(Operand operand, char parameter = 'x', int decimals = -1)
        {
            SetParameterChar(parameter);
            Func = operand;
            SetDecimals(decimals);
        }

        /// <value>
        /// Делегат с методом вычисления функции от значения
        /// </value>
        public Func<double, double> GetFunc => Func.GetDelegate(Decimals);
        /// <value>
        /// Производная исходной функции
        /// </value>
        public Function GetDerivative => new Function(Func.Derivative().Simplify(-1), ParameterChar, Decimals);
        /// <value>
        /// Делегат с методом вычисления производной исходной функции от значения
        /// </value>
        public Func<double, double> GetDerivativeFunc => GetDerivative.GetFunc;

        /// <summary>
        /// Возвращает строку, представляющую текущую функцию.
        /// </summary>
        /// <returns>Строковое представление функции</returns>
        public override string ToString() => Func.Simplify(Decimals).ToString(ParameterChar);

        public static Function operator -(Function func) => new Function(-func.Func, func.ParameterChar, func.Decimals);
        public static Function operator +(Function left, Function right)
        {
            Operand operand = left.Func + right.Func;
            char parameter = left.ParameterChar;
            int decimals = Math.Min(left.Decimals, right.Decimals);
            return new Function(operand, parameter, decimals);
        }
        public static Function operator -(Function left, Function right)
        {
            Operand operand = left.Func - right.Func;
            char parameter = left.ParameterChar;
            int decimals = Math.Min(left.Decimals, right.Decimals);
            return new Function(operand, parameter, decimals);
        }
        public static Function operator *(Function left, Function right)
        {
            Operand operand = left.Func * right.Func;
            char parameter = left.ParameterChar;
            int decimals = Math.Min(left.Decimals, right.Decimals);
            return new Function(operand, parameter, decimals);
        }
        public static Function operator /(Function left, Function right)
        {
            Operand operand = left.Func / right.Func;
            char parameter = left.ParameterChar;
            int decimals = Math.Min(left.Decimals, right.Decimals);
            return new Function(operand, parameter, decimals);
        }
        /// <summary>
        /// Подставляет исходную функцию в передаваемую
        /// </summary>
        /// <param name="other">Передаваемая функция</param>
        /// <returns>Переданная функция с подставленной исходной</returns>
        public Function SubstituteTo(Function other)
        {
            Operand operand = other.Func.Substitute(Func);
            char parameter = other.ParameterChar;
            int decimals = Math.Min(other.Decimals, Decimals);
            return new Function(operand, parameter, decimals);
        }
    }
}
