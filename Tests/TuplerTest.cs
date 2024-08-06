namespace Dynamitey.Tests
{
    [TestFixture]
    public class TuplerTest
    {

        [Test]
        public void DynamicCreateTypedTuple()
        {
            object tup = Tupler.Create(1, "2", "3", 4);

            var tup2 = Tuple.Create(1, "2", "3", 4);

            Assert.That(tup, Is.TypeOf(tup2.GetType()));

            Assert.That(tup, Is.EqualTo(tup2));
        }

        [Test]
        public void DynamicCreateTypedTuple8()
        {
            object tup = Tupler.Create(1, "2", "3", 4,
                    5, 6, 7, "8");

            var tup2 = Tuple.Create(1, "2", "3", 4, 5, 6, 7, "8");

            Assert.That(tup,Is.TypeOf(tup2.GetType()));

            Assert.That(tup, Is.EqualTo(tup2));
        }

        [Test]
        public void DynamicCreateLongTypedTuple()
        {
            object tup = Tupler.Create(1, "2", "3", 4,
                    5, 6, 7, "8", "9", 10, "11", 12);

            var tup2 = new Tuple<int, string, string, int, int, int, int, Tuple<string, string, int, string, int>>(
                1, "2", "3", 4,
                5, 6, 7, Tuple.Create("8", "9", 10, "11", 12)
                );

            Assert.That(tup, Is.TypeOf(tup2.GetType()));

            Assert.That(tup, Is.EqualTo(tup2));
        }

        [Test]
        public void DynamicTupleSize()
        {
            var tup = Tuple.Create(1, 2, 3, 4, 5);

            Assert.That((object)Tupler.Size(tup),Is.EqualTo(5));
        }
        [Test]
        public void DynamicTupleSize8()
        {
            var tup = Tuple.Create(1, 2, 3, 4, 5,6,7,8);

            Assert.That((object)Tupler.Size(tup), Is.EqualTo(8));
        }
        [Test]
        public void DynamicTupleSize20()
        {
            var tup = Tupler.Create(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20);

            Assert.That((object)Tupler.Size(tup), Is.EqualTo(20));
        }

        [Test]
        public void DynamicTupleToList()
        {
            var tup =Tuple.Create(1, 2, 3, 4, 5);
            var exp=Enumerable.Range(1,5).ToList();
            Assert.That((object)Tupler.ToList(tup),Is.EqualTo(exp));

        }

        [Test]
        public void DynamicTupleToList8()
        {
            var tup = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
            var exp = Enumerable.Range(1, 8).ToList();
            Assert.That((object)Tupler.ToList(tup), Is.EqualTo(exp));
        }

        [Test]
        public void DynamicTupleToList20()
        {
            var tup = Tupler.Create(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);
            var exp = Enumerable.Range(1, 20).ToList();
            Assert.That((object)Tupler.ToList(tup), Is.EqualTo(exp));
        }



        [Test]
        public void DynamicListToTuple()
        {
            var exp = Enumerable.Range(1, 5).ToList();
            var tup = exp.ToTuple();
            Assert.That((object)Tupler.IsTuple(tup), Is.True);
            Assert.That((object)Tupler.ToList(tup), Is.EqualTo(exp));

        }

        [Test]
        public void DynamicListToTuplet8()
        {
            var exp = Enumerable.Range(1, 8).ToList();
            var tup = exp.ToTuple();
            Assert.That((object)Tupler.IsTuple(tup), Is.True);
            Assert.That((object)Tupler.ToList(tup), Is.EqualTo(exp));
        }

        [Test]
        public void DynamicListToTuple20()
        {
    
            var exp = Enumerable.Range(1, 20).ToList();
            var tup = exp.ToTuple();
            Assert.That((object)Tupler.IsTuple(tup), Is.True);
            Assert.That((object)Tupler.ToList(tup), Is.EqualTo(exp));
        }



        [Test]
        public void DynamicTupleIndex()
        {
            var tup = Tupler.Create(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);
            Assert.That((object)Tupler.Index(tup,5), Is.EqualTo(6));
        }

        [Test]
        public void DynamicTupleIndex7()
        {
            var tup = Tupler.Create(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);
            Assert.That((object)Tupler.Index(tup, 7), Is.EqualTo(8));
        }

        [Test]
        public void DynamicTupleIndex19()
        {
            var tup = Tupler.Create(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);
            Assert.That((object)Tupler.Index(tup, 19), Is.EqualTo(20));
        }
    }
}
