namespace Rhythm.Extensions.Mapping {
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	internal static class ReflectionHelper {
		public static MemberInfo ToMember(this Expression expression) {
			var memberExpression = GetMemberExpression(expression);

			if (memberExpression == null) {
				return null;
			}

			return memberExpression.Member;
		}

		private static MemberExpression GetMemberExpression(Expression expression) {
			MemberExpression memberExpression = null;

			if (expression.NodeType == ExpressionType.Convert) {
				var body = (UnaryExpression)expression;
				memberExpression = body.Operand as MemberExpression;
			} else if (expression.NodeType == ExpressionType.MemberAccess) {
				memberExpression = expression as MemberExpression;
			}

			return memberExpression;
		}

		public static IList<Type> GetHierarchy(this Type type) {
			var types = new List<Type> { type };

			var parentType = type.BaseType;

			while (parentType != null) {
				types.Add(parentType);
				parentType = parentType.BaseType;
			}

			types.Reverse();

			return types;
		}
	}
}