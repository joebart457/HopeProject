using Hope.Compiler.Models.Enums;
using Hope.Compiler.Models.Instructions;
using System.Linq.Expressions;
using System.Reflection;

namespace Hope.Compiler._Parser
{
    internal class RuleBuilder
    {
        private ParsingRule? _previous;
        private List<ParsingRule> _rules = new();

        public List<ParsingRule> GetRules() => _rules;
        public RuleBuilder Register<Ty>(string prefix) where Ty: InstructionBase
        {
            _previous = new ParsingRule(typeof(Ty), prefix);
            _rules.Add(_previous);
            return this;
        }

        public RuleBuilder WithArgument<TSource>(ArgumentType argumentType, Expression<Func<TSource, object>> propertyLambda)
        {
            CheckPrevious();
            var propertyInfo = GetPropertyInfo(propertyLambda);
            _previous!.AddArgument(argumentType, propertyInfo);
            return this;
        }

        private void CheckPrevious()
        {
            if (_previous == null) throw new Exception("expect a call to Register before configuration method is called");
        }

        private PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression? member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo? propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                (type.ReflectedType == null || !type.IsSubclassOf(propInfo.ReflectedType!)))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

    }
}
