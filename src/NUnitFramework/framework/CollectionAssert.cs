using System;
using System.Collections;

namespace NUnit.Framework
{
	#region "Asserter Base Types"

	#region BaseSubsetAsserter
	public class BaseSubsetAsserter : CollectionCollectionAsserter
	{
		public BaseSubsetAsserter( ICollection set1, ICollection set2, string message, params object[] args ) : base(set1, set2, message, args)
		{
		}

		public override bool Test()
		{
			foreach(object set2Obj in set2)
			{
				bool found = false;

				foreach(object set1Obj in set1)
				{
					if (set2Obj.Equals(set1Obj))
					{
						found = true;
					}
				}

				if (!found)
				{
					return false;
				}
			}
			return true;
		}

	}
	#endregion

	#region BaseCollectionEquivalentAsserter
	public class BaseCollectionEquivalentAsserter : CollectionCollectionAsserter
	{
		public BaseCollectionEquivalentAsserter( ICollection set1, ICollection set2, string message, params object[] args ) : base(set1, set2, message, args)
		{
		}

		public override bool Test()
		{
			bool found = false;

			foreach(object set1Obj in set1)
			{
				found = false;
				foreach(object set2Obj in set2)
				{
					if (set1Obj.Equals(set2Obj))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					FailureMessage.AddLine("\tAn item from set1 was not found in set2.");
					return false;
				}
			}

			foreach(object set2Obj in set2)
			{
				found = false;
				foreach(object set1Obj in set1)
				{
					if (set1Obj.Equals(set2Obj))
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					FailureMessage.AddLine("\tAn item from set2 was not found in set1.");
					return false;
				}
			}

			return true;
		}
	}

	#endregion

	#region BaseCollectionEqualAsserter
	public class BaseCollectionEqualAsserter : CollectionsComparerAsserter
	{
		public BaseCollectionEqualAsserter( ICollection set1, ICollection set2, IComparer comparer, string message, params object[] args ) : base(set1, set2, comparer, message, args)
		{
		}

		public override bool Test()
		{
			int set1iteration = 0;
			int set2iteration = 0;
			bool isObjectsSame = false;

			if (set1.Count != set2.Count)
			{
				FailureMessage.AddLine("\tset1 and set2 do not have equal Count properties.");
				return false;
			}

			foreach(object set1Obj in set1)
			{
				set2iteration = 0;
				set1iteration += 1;

				foreach(object set2Obj in set2)
				{
					set2iteration += 1;

					if (set2iteration > set1iteration) break;
					if (set2iteration == set1iteration)
					{
						if (comparer == null)
							isObjectsSame = set1Obj.Equals(set2Obj);
						else
							isObjectsSame = comparer.Compare(set1Obj, set2Obj).Equals(0);

						if (!isObjectsSame)
						{
							FailureMessage.AddLine("\tset1 and set2 are not equal at index {0}",set1iteration);
							return false;
						}
						break;
					}
				}
			}
			return true;
		}

	}
	#endregion

	#region CollectionsComparerAsserter
	public class CollectionsComparerAsserter : CollectionCollectionAsserter
	{
		protected IComparer comparer;

		public CollectionsComparerAsserter( ICollection set1, ICollection set2, IComparer comparer, string message, params object[] args ) : base( set1, set2, message, args ) 
		{
			this.comparer = comparer;
		}
	}
	#endregion

	#region CollectionCollectionAsserter
	public class CollectionCollectionAsserter : CollectionAsserter
	{
		protected ICollection set2;

		public CollectionCollectionAsserter( ICollection set1, ICollection set2, string message, params object[] args ) : base( set1, message, args ) 
		{
			this.set2 = set2;
		}
	}
	#endregion

	#region CollectionObjectAsserter
	public class CollectionObjectAsserter : CollectionAsserter
	{
		protected object actual;

		public CollectionObjectAsserter( ICollection set1, object actual, string message, params object[] args ) : base( set1, message, args ) 
		{
			this.actual = actual;
		}
	}
	#endregion

	#region CollectionAsserter
	public class CollectionAsserter : AbstractAsserter
	{
		protected ICollection set1;
		protected string failMsg = "";

		public CollectionAsserter( ICollection set1, string message, params object[] args ) : base( message, args ) 
		{
			this.set1 = set1;
		}
	}
	#endregion

	#endregion

	#region Asserters

	#region ItemsOfTypeAsserter
	public class ItemsOfTypeAsserter : CollectionObjectAsserter
	{
		public ItemsOfTypeAsserter( ICollection set1, Type actual, string message, params object[] args ) : base(set1, actual, message, args)
		{
		}

		public override bool Test()
		{
			foreach(object loopObj in set1)
			{
				if (!loopObj.GetType().Equals(actual))
				{
					CreateMessage();
					return false;
				}
			}
			return true;
		}
		protected void CreateMessage()
		{
			FailureMessage.AddLine("\tAll objects are not of actual type.");
			FailureMessage.AddLine("\t{0} {1}","set1.Count:",set1.Count.ToString());
			FailureMessage.AddLine("\t{0} {1}","actual:",((Type)this.actual).Name);
		}
	}

	#endregion

	#region ItemsNotNullAsserter
	public class ItemsNotNullAsserter : CollectionAsserter
	{
		public ItemsNotNullAsserter( ICollection set1, string message, params object[] args ) : base(set1, message, args)
		{
		}

		public override bool Test()
		{
			foreach(object loopObj in set1)
			{
				if (loopObj == null)
				{
					CreateMessage();
					return false;
				}
			}
			return true;
		}
		protected void CreateMessage()
		{
			FailureMessage.AddLine("\tAt least one object is null.");
			FailureMessage.AddLine("\t{0} {1}","set1.Count:",set1.Count.ToString());
		}
	}

	#endregion

	#region ItemsUniqueAsserter
	public class ItemsUniqueAsserter : CollectionAsserter
	{
		public ItemsUniqueAsserter( ICollection set1, string message, params object[] args ) : base(set1, message, args)
		{
		}

		public override bool Test()
		{
			foreach(object loopObj in set1)
			{
				bool foundOnce = false;
				foreach(object innerObj in set1)
				{
					if (loopObj.Equals(innerObj))
					{
						if (foundOnce)
						{
							CreateMessage();
							return false;
						}
						else
						{
							foundOnce = true;
						}
					}
				}
			}
			return true;
		}
		protected void CreateMessage()
		{
			FailureMessage.AddLine("\tAt least one object is not unique within set1.");
			FailureMessage.AddLine("\t{0} {1}","set1.Count:",set1.Count.ToString());
		}
	}

	#endregion

	#region CollectionContains
	public class CollectionContains : CollectionObjectAsserter
	{
		public CollectionContains( ICollection set1, object actual, string message, params object[] args ) : base(set1, actual, message, args)
		{
		}

		public override bool Test()
		{
			foreach(object loopObj in set1)
			{
				if (!loopObj.Equals(actual))
				{
					return true;
				}
			}
			CreateMessage();
			return true;
		}
		protected void CreateMessage()
		{
			FailureMessage.AddLine("\tThe actual object was not found in set1.");
			FailureMessage.AddLine("\t{0} {1}","set1.Count:",set1.Count.ToString());
			FailureMessage.AddActualLine(actual.ToString());
		}
	}

	#endregion

	#region CollectionNotContains
	public class CollectionNotContains : CollectionObjectAsserter
	{
		public CollectionNotContains( ICollection set1, object actual, string message, params object[] args ) : base(set1, actual, message, args)
		{
		}

		public override bool Test()
		{
			foreach(object loopObj in set1)
			{
				if (loopObj.Equals(actual))
				{
					CreateMessage();
					return false;
				}
			}
			return true;
		}
		protected void CreateMessage()
		{
			FailureMessage.AddLine("\tThe actual object was found in set1.");
			FailureMessage.AddLine("\t{0} {1}","set1.Count:",set1.Count.ToString());
			FailureMessage.AddActualLine(actual.ToString());
		}
	}

	#endregion

	#region CollectionEqualAsserter
	public class CollectionEqualAsserter : BaseCollectionEqualAsserter
	{
		public CollectionEqualAsserter( ICollection set1, ICollection set2, IComparer comparer, string message, params object[] args ) : base(set1, set2, comparer, message, args)
		{
		}

		public override bool Test()
		{
			if (base.Test())
			{
				return true;
			}
			else
			{
				CreateMessage();
				return false;
			}
		}


		private void CreateMessage()
		{
		}
	}

	#endregion

	#region CollectionNotEqualAsserter
	public class CollectionNotEqualAsserter : BaseCollectionEqualAsserter
	{
		public CollectionNotEqualAsserter( ICollection set1, ICollection set2, IComparer comparer, string message, params object[] args ) : base(set1, set2, comparer, message, args)
		{
		}

		public override bool Test()
		{
			if (base.Test())
			{
				CreateMessage();
				return false;
			}
			else
			{
				return true;
			}
		}


		private void CreateMessage()
		{
		}
	}

	#endregion

	#region CollectionEquivalentAsserter
	public class CollectionEquivalentAsserter : BaseCollectionEquivalentAsserter
	{
		public CollectionEquivalentAsserter( ICollection set1, ICollection set2, string message, params object[] args ) : base(set1, set2, message, args)
		{
		}

		public override bool Test()
		{
			if (base.Test())
			{
				return true;
			}
			else
			{
				CreateMessage();
				return false;
			}
		}

		private void CreateMessage()
		{
		}
	}

	#endregion

	#region CollectionNotEquivalentAsserter
	public class CollectionNotEquivalentAsserter : BaseCollectionEquivalentAsserter
	{
		public CollectionNotEquivalentAsserter( ICollection set1, ICollection set2, string message, params object[] args ) : base(set1, set2, message, args)
		{
		}

		public override bool Test()
		{
			if (!base.Test())
			{
				return true;
			}
			else
			{
				CreateMessage();
				return false;
			}
		}

		private void CreateMessage()
		{
		}
	}

	#endregion

	#region SubsetAsserter
	public class SubsetAsserter : BaseSubsetAsserter
	{
		public SubsetAsserter( ICollection set1, ICollection set2, string message, params object[] args ) : base(set1, set2, message, args)
		{
		}

		public override bool Test()
		{
			if (base.Test())
			{
				return true;
			}
			else
			{
				CreateMessage();
				return false;
			}
		}

		private void CreateMessage()
		{
		}
	}

	#endregion

	#region NotSubsetAsserter
	public class NotSubsetAsserter : BaseSubsetAsserter
	{
		public NotSubsetAsserter( ICollection set1, ICollection set2, string message, params object[] args ) : base(set1, set2, message, args)
		{
		}

		public override bool Test()
		{
			if (!base.Test())
			{
				return true;
			}
			else
			{
				CreateMessage();
				return false;
			}
		}

		private void CreateMessage()
		{
		}
	}

	#endregion

	#endregion

	public class CollectionAssert
	{

		#region AllItemsAreInstancesOfType
		/// <summary>
		/// Asserts that all items contained in collection are of the type specified by expectedType.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
		public static void AllItemsAreInstancesOfType (ICollection collection, Type expectedType)
		{
			AllItemsAreInstancesOfType(collection, expectedType, string.Empty, null);
		}

		/// <summary>
		/// Asserts that all items contained in collection are of the type specified by expectedType.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AllItemsAreInstancesOfType (ICollection collection, Type expectedType, string message)
		{
			AllItemsAreInstancesOfType(collection, expectedType, message, null);
		}

		/// <summary>
		/// Asserts that all items contained in collection are of the type specified by expectedType.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="expectedType">System.Type that all objects in collection must be instances of</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AllItemsAreInstancesOfType (ICollection collection, Type expectedType, string message, params object[] args)
		{
			Assert.DoAssert(new ItemsOfTypeAsserter(collection, expectedType, message, args));
		}
		#endregion

		#region AllItemsAreNotNull

		/// <summary>
		/// Asserts that all items contained in collection are not equal to null.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		public static void AllItemsAreNotNull (ICollection collection) 
		{
			AllItemsAreNotNull(collection, string.Empty, null);
		}

		/// <summary>
		/// Asserts that all items contained in collection are not equal to null.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AllItemsAreNotNull (ICollection collection, string message) 
		{
			AllItemsAreNotNull(collection, message, null);
		}

		/// <summary>
		/// Asserts that all items contained in collection are not equal to null.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AllItemsAreNotNull (ICollection collection, string message, params object[] args) 
		{
			Assert.DoAssert(new ItemsNotNullAsserter(collection, message, args));
		}
		#endregion

		#region AllItemsAreUnique

		/// <summary>
		/// Ensures that every object contained in collection exists within the collection
		/// once and only once.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		public static void AllItemsAreUnique (ICollection collection) 
		{
			AllItemsAreUnique(collection, string.Empty, null);
		}

		/// <summary>
		/// Ensures that every object contained in collection exists within the collection
		/// once and only once.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AllItemsAreUnique (ICollection collection, string message) 
		{
			AllItemsAreUnique(collection, message, null);
		}
		
		/// <summary>
		/// Ensures that every object contained in collection exists within the collection
		/// once and only once.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AllItemsAreUnique (ICollection collection, string message, params object[] args) 
		{
			Assert.DoAssert(new ItemsUniqueAsserter(collection, message, args));
		}
		#endregion

		#region AreEqual

		/// <summary>
		/// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
		/// and contain the exact same objects in the same order.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		public static void AreEqual (ICollection expected, ICollection actual) 
		{
			AreEqual(expected, actual, null, string.Empty, null);
		}

		/// <summary>
		/// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
		/// and contain the exact same objects in the same order.
		/// If comparer is not null then it will be used to compare the objects.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
		public static void AreEqual (ICollection expected, ICollection actual, IComparer comparer) 
		{
			AreEqual(expected, actual, comparer, string.Empty, null);
		}

		/// <summary>
		/// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
		/// and contain the exact same objects in the same order.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AreEqual (ICollection expected, ICollection actual, string message) 
		{
			AreEqual(expected, actual, null, message, null);
		}

		/// <summary>
		/// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
		/// and contain the exact same objects in the same order.
		/// If comparer is not null then it will be used to compare the objects.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AreEqual (ICollection expected, ICollection actual, IComparer comparer, string message) 
		{
			AreEqual(expected, actual, comparer, message, null);
		}

		/// <summary>
		/// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
		/// and contain the exact same objects in the same order.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AreEqual (ICollection expected, ICollection actual, string message, params object[] args) 
		{
			AreEqual(expected, actual, null, message, args);
		}

		/// <summary>
		/// Asserts that expected and actual are exactly equal.  The collections must have the same count, 
		/// and contain the exact same objects in the same order.
		/// If comparer is not null then it will be used to compare the objects.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AreEqual (ICollection expected, ICollection actual, IComparer comparer, string message, params object[] args) 
		{
			Assert.DoAssert(new CollectionEqualAsserter(expected, actual, comparer, message, args));
		}
		#endregion

		#region AreEquivalent

		/// <summary>
		/// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		public static void AreEquivalent (ICollection expected, ICollection actual) 
		{
			AreEquivalent(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AreEquivalent (ICollection expected, ICollection actual, string message) 
		{
			AreEquivalent(expected, actual, message, null);
		}

		/// <summary>
		/// Asserts that expected and actual are equivalent, containing the same objects but the match may be in any order.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AreEquivalent (ICollection expected, ICollection actual, string message, params object[] args) 
		{
			Assert.DoAssert(new CollectionEquivalentAsserter(expected, actual, message, args));
		}
		#endregion

		#region AreNotEqual

		/// <summary>
		/// Asserts that expected and actual are not exactly equal.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		public static void AreNotEqual (ICollection expected, ICollection actual)
		{
			AreNotEqual(expected, actual, null, string.Empty, null);
		}

		/// <summary>
		/// Asserts that expected and actual are not exactly equal.
		/// If comparer is not null then it will be used to compare the objects.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
		public static void AreNotEqual (ICollection expected, ICollection actual, IComparer comparer)
		{
			AreNotEqual(expected, actual, comparer, string.Empty, null);
		}

		/// <summary>
		/// Asserts that expected and actual are not exactly equal.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AreNotEqual (ICollection expected, ICollection actual, string message)
		{
			AreNotEqual(expected, actual, null, message, null);
		}

		/// <summary>
		/// Asserts that expected and actual are not exactly equal.
		/// If comparer is not null then it will be used to compare the objects.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AreNotEqual (ICollection expected, ICollection actual, IComparer comparer, string message)
		{
			AreNotEqual(expected, actual, comparer, message, null);
		}

		/// <summary>
		/// Asserts that expected and actual are not exactly equal.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AreNotEqual (ICollection expected, ICollection actual, string message, params object[] args) 
		{
			AreNotEqual(expected, actual, null, message, args);
		}

		/// <summary>
		/// Asserts that expected and actual are not exactly equal.
		/// If comparer is not null then it will be used to compare the objects.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="comparer">The IComparer to use in comparing objects from each ICollection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AreNotEqual (ICollection expected, ICollection actual, IComparer comparer, string message, params object[] args)
		{
			Assert.DoAssert(new CollectionNotEqualAsserter(expected, actual, comparer, message, args));
		}
		#endregion

		#region AreNotEquivalent

		/// <summary>
		/// Asserts that expected and actual are not equivalent.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		public static void AreNotEquivalent (ICollection expected, ICollection actual)
		{
			AreNotEquivalent(expected, actual, string.Empty, null);
		}

		/// <summary>
		/// Asserts that expected and actual are not equivalent.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void AreNotEquivalent (ICollection expected, ICollection actual, string message)
		{
			AreNotEquivalent(expected, actual, message, null);
		}

		/// <summary>
		/// Asserts that expected and actual are not equivalent.
		/// </summary>
		/// <param name="expected">The first ICollection of objects to be considered</param>
		/// <param name="actual">The second ICollection of objects to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void AreNotEquivalent (ICollection expected, ICollection actual, string message, params object[] args)
		{
			Assert.DoAssert(new CollectionNotEquivalentAsserter(expected, actual, message, args));
		}
		#endregion

		#region Contains

		/// <summary>
		/// Asserts that collection contains actual as an item.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="actual">Object to be found within collection</param>
		public static void Contains (ICollection collection, Object actual)
		{
			Contains(collection, actual, string.Empty, null);
		}

		/// <summary>
		/// Asserts that collection contains actual as an item.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="actual">Object to be found within collection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void Contains (ICollection collection, Object actual, string message)
		{
			Contains(collection, actual, message, null);
		}

		/// <summary>
		/// Asserts that collection contains actual as an item.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="actual">Object to be found within collection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void Contains (ICollection collection, Object actual, string message, params object[] args)
		{
			Assert.DoAssert(new CollectionContains(collection, actual, message, args));
		}
		#endregion

		#region DoesNotContain

		/// <summary>
		/// Asserts that collection does not contain actual as an item.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="actual">Object that cannot exist within collection</param>
		public static void DoesNotContain (ICollection collection, Object actual)
		{
			DoesNotContain(collection, actual, string.Empty, null);
		}

		/// <summary>
		/// Asserts that collection does not contain actual as an item.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="actual">Object that cannot exist within collection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void DoesNotContain (ICollection collection, Object actual, string message)
		{
			DoesNotContain(collection, actual, message, null);
		}

		/// <summary>
		/// Asserts that collection does not contain actual as an item.
		/// </summary>
		/// <param name="collection">ICollection of objects to be considered</param>
		/// <param name="actual">Object that cannot exist within collection</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void DoesNotContain (ICollection collection, Object actual, string message, params object[] args)
		{
			Assert.DoAssert(new CollectionNotContains(collection, actual, message, args));
		}
		#endregion

		#region IsNotSubsetOf

		/// <summary>
		/// Asserts that superset is not a subject of subset.
		/// </summary>
		/// <param name="subset">The ICollection superset to be considered</param>
		/// <param name="superset">The ICollection subset to be considered</param>
		public static void IsNotSubsetOf (ICollection subset, ICollection superset)
		{
			IsNotSubsetOf(subset, superset, string.Empty, null);
		}

		/// <summary>
		/// Asserts that superset is not a subject of subset.
		/// </summary>
		/// <param name="subset">The ICollection superset to be considered</param>
		/// <param name="superset">The ICollection subset to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void IsNotSubsetOf (ICollection subset, ICollection superset, string message)
		{
			IsNotSubsetOf(subset, superset, message, null);
		}

		/// <summary>
		/// Asserts that superset is not a subject of subset.
		/// </summary>
		/// <param name="subset">The ICollection superset to be considered</param>
		/// <param name="superset">The ICollection subset to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void IsNotSubsetOf (ICollection subset, ICollection superset, string message, params object[] args)
		{
			Assert.DoAssert(new NotSubsetAsserter(subset, superset, message, args));
		}
		#endregion

		#region IsSubsetOf

		/// <summary>
		/// Asserts that superset is a subset of subset.
		/// </summary>
		/// <param name="subset">The ICollection superset to be considered</param>
		/// <param name="superset">The ICollection subset to be considered</param>
		public static void IsSubsetOf (ICollection subset, ICollection superset)
		{
			IsSubsetOf(subset, superset, string.Empty, null);
		}

		/// <summary>
		/// Asserts that superset is a subset of subset.
		/// </summary>
		/// <param name="subset">The ICollection superset to be considered</param>
		/// <param name="superset">The ICollection subset to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		public static void IsSubsetOf (ICollection subset, ICollection superset, string message)
		{
			IsSubsetOf(subset, superset, message, null);
		}

		/// <summary>
		/// Asserts that superset is a subset of subset.
		/// </summary>
		/// <param name="subset">The ICollection superset to be considered</param>
		/// <param name="superset">The ICollection subset to be considered</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		public static void IsSubsetOf (ICollection subset, ICollection superset, string message, params object[] args)
		{
			Assert.DoAssert(new SubsetAsserter(subset, superset, message, args));
		}
		#endregion

	}
}

