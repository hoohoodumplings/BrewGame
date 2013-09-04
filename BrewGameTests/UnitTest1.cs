using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BrewGame;

namespace BrewGameTests
{

    [TestClass]
    public class TestFermVessel
    {
        [TestMethod]
        public void TestAdd()
        {
           
            Recipe testRecipe = new Recipe();
            FermVessel testFermVessel = new FermVessel();

            // Able to add recipe successfully?
            bool success = testFermVessel.Add(testRecipe.output,
                testRecipe.brewQty, testRecipe.time);
            Assert.IsTrue(success);

            Assert.AreEqual(testRecipe.output,
                testFermVessel.storedBrew);

        }

        [TestMethod]
        public void TestAge()
        {
            Recipe testRecipe = new Recipe();
            FermVessel testFermVessel = new FermVessel();
            int starting = testRecipe.time;

            testFermVessel.Add(testRecipe.output,
                testRecipe.brewQty, testRecipe.time);

            testFermVessel.Age(1);

            Assert.AreEqual(starting - 1, testFermVessel.timeRemaining);

        }

        [TestMethod]
        public void TestEmpty()
        {

            Recipe testRecipe = new Recipe();
            FermVessel testFermVessel = new FermVessel();
            int starting = testRecipe.time;

            testFermVessel.Add(testRecipe.output,
                testRecipe.brewQty, testRecipe.time);

            testFermVessel.Empty();

            Assert.IsTrue(testFermVessel.isEmpty());

        }
    }
    
    [TestClass]
    public class TestBrewery
    {
        [TestMethod]
        public void TestAdd()
        {

            Brewery testBrewery = new Brewery("Casey");
            Ingredient testIngredient = new Ingredient("test", 1);
            Brew testBrew = new Brew("american lager");
            FermVessel testFermVessel = new FermVessel();

            // Empty test
            Assert.AreEqual(0, testBrewery.ingBag.Count);
            Assert.AreEqual(0, testBrewery.brewBag.Count);
            Assert.AreEqual(0, testBrewery.fvBag.Count);

            // Add ingredient
            testBrewery.Add(testIngredient, 20);
            Assert.AreEqual(1, testBrewery.ingBag.Count);
            Assert.AreEqual(20, testBrewery.ingQty[0]);

            testBrewery.Add(new Ingredient("test", 1), 20);
            Assert.AreEqual(1, testBrewery.ingBag.Count);
            Assert.AreEqual(40, testBrewery.ingQty[0]);

            testBrewery.Add(new Ingredient("water", 1), 20);
            Assert.AreEqual(2, testBrewery.ingBag.Count);
            Assert.AreEqual(40, testBrewery.ingQty[0]);
            Assert.AreEqual(20, testBrewery.ingQty[1]);

            // Add brew
            testBrewery.Add(testBrew, 20);
            Assert.AreEqual(1, testBrewery.brewBag.Count);
            Assert.AreEqual(20, testBrewery.brewQty[0]);

            testBrewery.Add(new Brew("american lager"), 20);
            Assert.AreEqual(1, testBrewery.brewBag.Count);
            Assert.AreEqual(40, testBrewery.brewQty[0]);

            Brew brew2 = new Brew("test", 1);
            testBrewery.Add(brew2, 20);
            Assert.AreEqual(2, testBrewery.brewBag.Count);
            Assert.AreEqual(40, testBrewery.brewQty[0]);
            Assert.AreEqual(20, testBrewery.brewQty[1]);

            // Add fermentation vessel
            testBrewery.Add(testFermVessel);
            Assert.AreEqual(1, testBrewery.fvBag.Count);

            testBrewery.Add(new FermVessel());
            Assert.AreEqual(2, testBrewery.fvBag.Count);
        }

        [TestMethod]
        public void TestRemoveIngredient()
        {
            Brewery testBrewery = new Brewery("Casey");
            Ingredient testIngredient = new Ingredient("test", 1);
            bool success;

            // Start with 10
            testBrewery.Add(testIngredient, 10);

            // Remove 5
            success = testBrewery.Remove(new Ingredient("test", 1), 5);
            Assert.IsTrue(success);
            Assert.AreEqual(5, testBrewery.ingQty[0]);

            // Try to remove another 10
            success = testBrewery.Remove(new Ingredient("test", 1), 10);
            Assert.IsFalse(success);
            Assert.AreEqual(5, testBrewery.ingQty[0]);

            // Remove the final 5 and the entry is deleted
            success = testBrewery.Remove(new Ingredient("test", 1), 5);
            Assert.IsTrue(success);
            Assert.AreEqual(0, testBrewery.ingBag.Count);

        }

        [TestMethod]
        public void TestRemoveBrew()
        {
            Brewery testBrewery = new Brewery("Casey");
            Brew testBrew = new Brew("test", 1);
            bool success;

            // Start with 10
            testBrewery.Add(testBrew, 10);

            // Remove 5
            success = testBrewery.Remove(new Brew("test", 1), 5);
            Assert.IsTrue(success);
            Assert.AreEqual(5, testBrewery.brewQty[0]);

            // Try to remove another 10
            success = testBrewery.Remove(new Brew("test", 1), 10);
            Assert.IsFalse(success);
            Assert.AreEqual(5, testBrewery.brewQty[0]);

            // Remove the final 5 and the entry is deleted
            success = testBrewery.Remove(new Brew("test", 1), 5);
            Assert.IsTrue(success);
            Assert.AreEqual(0, testBrewery.brewBag.Count);

        }

        [TestMethod]
        public void TestRemoveFermVessel()
        {
            Brewery testBrewery = new Brewery("Casey");
            FermVessel testFermVessel = new FermVessel();

            testBrewery.Add(testFermVessel);
            Assert.AreEqual(1, testBrewery.fvBag.Count);

            testBrewery.RemoveFermVessel(0);
            Assert.AreEqual(0, testBrewery.fvBag.Count);
        }

        [TestMethod]
        public void TestAgeAll()
        {
            Brewery testBrewery = new Brewery("Casey");

            testBrewery.Add(new FermVessel());
            testBrewery.Add(new FermVessel());

            testBrewery.fvBag[0].Add(new Brew(), 1, 10);
            testBrewery.fvBag[1].Add(new Brew(), 1, 5);

            Assert.AreEqual(10, testBrewery.fvBag[0].timeRemaining);
            Assert.AreEqual(5, testBrewery.fvBag[1].timeRemaining);

            testBrewery.AgeAll(1);

            Assert.AreEqual(9, testBrewery.fvBag[0].timeRemaining);
            Assert.AreEqual(4, testBrewery.fvBag[1].timeRemaining);
        }
    }

    [TestClass]
    public class TestIngredient
    {
        [TestMethod]
        public void TestGetAllIngredients()
        {
            List<Ingredient> list = Ingredient.GetAllIngredients();
            Assert.AreEqual(Ingredient.nIngredients, list.Count);
        }
    }
    
    [TestClass]
    public class TestCompany
    {
        [TestMethod]
        public void TestAddBrewery()
        {
            Company testCompany = new Company("testco", 1000);
            Brewery testBrewery = new Brewery();
            Location testLocation1 = new Location("testloc1", new int[] {1,1});
            Location testLocation2 = new Location("testloc2", new int[] {2,2});

            // Successfully add a brewery
            Assert.IsTrue(testCompany.Add(testBrewery, testLocation1));

            // Can't add to the same location
            Assert.IsFalse(testCompany.Add(testBrewery, testLocation1));

            // Still only 1
            Assert.AreEqual(1, testCompany.breweries.Count);

            // Can add to a new location
            Assert.IsTrue(testCompany.Add(testBrewery, testLocation2));

        }

        [TestMethod]
        public void TestAddRecipe()
        {
            Company testCompany = new Company("testco", 1000);
            Recipe testRecipe = new Recipe();

            // Starts at 0
            Assert.AreEqual(0, testCompany.recipes.Count);

            // Add 1
            Assert.IsTrue(testCompany.Add(testRecipe));
            Assert.AreEqual(1, testCompany.recipes.Count);

            // Add another
            Assert.IsTrue(testCompany.Add(testRecipe));
            Assert.AreEqual(2, testCompany.recipes.Count);
        }
    }
}
