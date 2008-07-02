// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org/?p=license&r=2.4.
// ****************************************************************

namespace NUnit.Framework
{
	using System;

	/// <example>
	/// [TestFixture]
	/// public class ExampleClass 
	/// {}
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	public class TestFixtureAttribute : Attribute
	{
		private string description;
        private Type[] typeArguments;

        public TestFixtureAttribute() { }
        public TestFixtureAttribute(params Type[] typeArguments)
        {
            this.typeArguments = typeArguments;
        }

		/// <summary>
		/// Descriptive text for this fixture
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

        public Type[] TypeArguments
        {
            get { return typeArguments; }
        }
	}
}
