namespace Dynamitey.Tests
{
    /// <summary>
    /// This is the craziest set of tests I've ever written in my life...
    /// </summary>
    [TestFixture]
    public class MimicTest
    {
        private class SubMimic : DynamicObjects.Mimic
        {
            public int Add(int x, int y)
            {
                return x + y;
            }

            public string Add(string x, string y)
            {
                return x + y;
            }
        }

        [Test]
        public void Get_Property()
        {
            dynamic mimic = new DynamicObjects.Mimic();
            dynamic result = mimic.I.Can.Get.Any.Property.I.Want.And.It.Wont.Blow.Up;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
        }

        [Test]
        public void Set_Property()
        {
            dynamic mimic = new DynamicObjects.Mimic();
            dynamic result = mimic.I.Can.Set.Any.Property.I.Want.And.It.Wont.Blow = "Up";
            Assert.That((object)result, Is.EqualTo("Up"));
        }

        [Test]
        public void Call_Method()
        {
            dynamic mimic = new DynamicObjects.Mimic();
            dynamic result = mimic.I.Can.Call.Any.Method.I.Want.And.It.Wont.Blow.Up();
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
        }

        [Test]
        public void Call_Method_With_Parameters()
        {
            dynamic mimic = new DynamicObjects.Mimic();
            dynamic result = mimic.I().Can().Call().Any().Method().I().Want().And().It().Wont().Blow().Up("And", "Any", "Parameter", "I", "Want", 1, 2, 3, 44.99m);
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
        }

        [Test]
        public void Get_Index()
        {
            dynamic mimic = new DynamicObjects.Mimic();
            dynamic result = mimic["I"]["Can"]["Get"]["Indexes"]["All"]["Day"]["Like"]["It"]["Aint"]["No"]["Thang"];
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
        }

        [Test]
        public void Set_Index()
        {
            dynamic mimic = new DynamicObjects.Mimic();
            dynamic result = mimic["I"]["Can"]["Set"]["Indexes"]["All"]["Day"]["Like"]["It"]["Aint"]["No"] = "Thang";
            Assert.That((object)result, Is.EqualTo("Thang"));
        }

        [Test]
        public void Cast()
        {
            dynamic mimic = new DynamicObjects.Mimic();

            int Int32 = mimic;
            Assert.That(Int32,Is.EqualTo(0));
            double Double = mimic;
            Assert.That(Double, Is.EqualTo(0.0d));
            float Float = mimic;
            Assert.That(Float, Is.EqualTo(0.0f));
            object Object = mimic;
            Assert.That(Object,  Is.TypeOf<DynamicObjects.Mimic>());
            Guid Guid = mimic;
            Assert.That(Guid, Is.EqualTo(Guid.Empty));
            DateTime DateTime = mimic;
            Assert.That(DateTime, Is.EqualTo(default(DateTime)));
        }

        [Test]
        public void Unary()
        {
            dynamic mimic = new DynamicObjects.Mimic();
            dynamic result;

            result = !mimic;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = ++mimic;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = --mimic;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic++;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic--;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic += 1;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic -= 1;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic /= 2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic *= 4;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic ^= true;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic |= true;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic &= false;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = mimic %= 5;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
        }

        [Test]
        public void Binary()
        {
            dynamic thing1 = new DynamicObjects.Mimic();
            dynamic thing2 = new DynamicObjects.Mimic();
            dynamic result;

            result = thing1 + thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = thing1 - thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = thing1 / thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = thing1 * thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = thing1 | thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = thing1 & thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = thing1 ^ thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
            result = thing1 % thing2;
            Assert.That((object)result, Is.TypeOf<DynamicObjects.Mimic>());
        }

        [Test]
        public void Inheritance_Int()
        {
            dynamic mimic = new SubMimic();
            int result = mimic.Add(2, 2);
            Assert.That(4, Is.EqualTo(result));
        }

        [Test]
        public void Inheritance_String()
        {
            dynamic mimic = new SubMimic();
            string result = mimic.Add("He", "llo");
            Assert.That("Hello", Is.EqualTo(result));
        }

        [Test]
        public void Inheritance_No_Match()
        {
            dynamic mimic = new SubMimic();
            int result = mimic.Add(1, "llo");
            Assert.That(default(int), Is.EqualTo(result));
        }
    }
}
