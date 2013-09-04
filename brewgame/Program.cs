using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrewGame
{
    class Program
    {
        public static Company company;
        public static List<Location> locations = new List<Location>();
        public static int time = 1;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to BrewGame.");
            
            // Start new game
            locations.Add(new Location("Testville, USA", new int[] { 5, 5 }));

            Console.WriteLine("\nEnter your company name.");
            company = new Company(Console.ReadLine(), 1000);
            
            // Start main loop
            MainLoop();

        }

        // ----- MainLoop ---------------------------------
        //
        // ------------------------------------------------
        public static void MainLoop()
        {
            bool cont = true;
            int selection;

            while (cont == true)
            {
                PrintHeader();
                Console.WriteLine("Select an option:");
                Console.WriteLine("1) Buy a brewery.");
                Console.WriteLine("2) Buy a recipe.");
                Console.WriteLine("3) Buy ingredients.");
                Console.WriteLine("4) Brew a recipe.");
                Console.WriteLine("5) Advance a week.");
                Console.WriteLine("Q) Quit BrewGame.");
                selection = GetUserInput(5);

                if (selection == -1) return;
                // note: this exits even if user inputs something like 76

                // NOTE THAT CASE #'s ARE -= 1 FROM IDX
                switch (selection)
                {
                    case 0:
                        // buy brewery
                        BuyBrewery();
                        break;
                    case 1:
                        // buy recipe
                        break;
                    case 2:
                        // buy ingredients
                        BuyIngredients();
                        break;
                    case 3:
                        // brew a recipe
                        break;
                    case 4:
                        // advance a week
                        AdvanceTime(1);
                        break;
                    default:
                        break;
                }
            }
        }

        // ----- PrintHeader ------------------------------
        //
        // Print the info header
        //
        // ------------------------------------------------
        public static void PrintHeader()
        {
            Console.Clear();
            Console.WriteLine("{0} | Week {1} | ${2}\n",
                    company.nm, time, company.cash);
        }

        // ----- AdvanceTime ------------------------------
        //
        // Advance the time counter
        //
        // ------------------------------------------------
        public static void AdvanceTime(int t)
        {
            // TODO - do this for each company when # players > 1
           
            // For each brewery...
            for (int i = 0; i < company.breweries.Count; i++)
            {
                company.breweries[i].AgeAll(t);
            }

            time += 1;
        }


        // ----- GetUserInput -----------------------------
        //
        // Select an item from a list
        // Note that this is just the input portion
        // Returns -1 if invalid selection or abort
        //
        // ------------------------------------------------
        public static int GetUserInput(int listCount)
        {
            int idx;
            const int EXIT = -1;

            string choice = Console.ReadLine();
            PrintHeader();

            if (choice.ToLower() == "q")
            {
                Console.WriteLine("Exiting...");
                Console.ReadKey();
                return EXIT;
            }

            try
            {
                idx = Convert.ToInt32(choice) - 1;
            }
            catch (FormatException e)
            {
                Console.WriteLine("Invalid selection.");
                Console.ReadKey();
                return EXIT;
            }

            // Exit if input is outside of range
            if (idx < 0 || idx >= listCount)
            {
                Console.WriteLine("Invalid selection: {0}.", idx+1);
                Console.ReadKey();
                return EXIT;
            }
            else
            {
                return idx;
            }
        }

        // ----- BuyIngredients ---------------------------
        //
        // Buy ingredients for a specific brewery
        //
        // ------------------------------------------------
        public static void BuyIngredients()
        {
            int breweryIdx, ingIdx, maxQty, px, qty;
            
            Brewery selectedBrewery;
            Location selectedLocation;
            Ingredient selectedIngredient;

            // List available breweries
            for (int i = 0; i < company.breweries.Count; i++)
            {
                Console.WriteLine("{0}) {1}",
                    i + 1, company.breweryLocations[i].nm);
            }
            Console.WriteLine("Q) Abort");
            
            // Prompt user for input
            breweryIdx = GetUserInput(company.breweries.Count);
            if (breweryIdx == -1) return;
            
            selectedBrewery = company.breweries[breweryIdx];
            selectedLocation = company.breweryLocations[breweryIdx];

            // List all available ingredients and prices
            List<Ingredient> ingList = Ingredient.GetAllIngredients();
            for (int i = 0; i < ingList.Count; i++)
            {
                Console.WriteLine("{0}) ${2} | {1}",
                    i + 1, ingList[i].nm, 
                    selectedLocation.GetPrice(ingList[i].nm));
            }
            
            // Prompt user for ingredient input
            ingIdx = GetUserInput(ingList.Count);
            if (ingIdx == -1) return;
            selectedIngredient = ingList[ingIdx];

            // Prompt user for quantity
            px = selectedLocation.GetPrice(selectedIngredient.nm);
            maxQty = company.cash / px;
            Console.WriteLine("How many \"{0}\" for {1} each?",
                selectedIngredient.nm, px);
            Console.WriteLine("(0-{0})", maxQty);
            try
            {
                do
                {
                    qty = Convert.ToInt32(Console.ReadLine());
                    if (qty < 0 || qty > maxQty)
                    {
                        Console.WriteLine("Outside acceptable range.");
                        Console.ReadKey();
                    }
                }
                while (qty < 0 || qty > maxQty);
                
            }
            catch (FormatException e)
            {
                Console.WriteLine("Invalid selection.");
                Console.ReadKey();
                return;
            }

            // Charge the company's cash and add the ingredient
            if (!company.Charge(px * qty))
            {
                Console.WriteLine("Insufficient funds.");
                Console.ReadKey();
                return;
            }
            else
            {
                selectedBrewery.Add(selectedIngredient, qty);
                Console.WriteLine("Purchased {0} units of \"{1}\".",
                    qty, selectedIngredient.nm);
                Console.ReadKey();
            }
        }

        // Buy a brewery at a specific location
        public static void BuyBrewery()
        {

            int locIdx;

            PrintHeader();

            // Print location information
            for (int i = 0; i < locations.Count; i++)
            {
                Console.WriteLine("{0}) {1}: {2}",
                    i+1, locations[i].nm, locations[i].breweryCost);
            }
            Console.WriteLine("Q) Abort");

            // Prompt user for input
            locIdx = GetUserInput(locations.Count);
            if (locIdx == -1) return;
            
            // Verify no existing brewery at that location
            if (company.breweryLocations.Contains(locations[locIdx]))
            {
                Console.WriteLine("A brewery already exists at that location.");
                Console.ReadKey();
                return;
            }

            // Check company has enough cash and charge to co
            if (company.Charge(locations[locIdx].breweryCost))
            {
                // Add the brewery
                company.breweries.Add(new Brewery(company.nm));
                company.breweryLocations.Add(locations[locIdx]);
                Console.WriteLine("Brewery purchased in {0}.",
                    locations[locIdx].nm);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
                Console.ReadKey();
            }

        }
    }

    public class Location
    {
        public string nm { get; private set; }
        public int[] coords { get; private set; }
        public int breweryCost { get; private set; }
        private List<Ingredient> ingredients { get; set; }

        // Empty constructor
        public Location()
        {
            nm = "EMPTY";
            coords = new int[] {0,0};
            breweryCost = 500;
            ingredients = Ingredient.GetAllIngredients();
        }

        // Basic constructor
        public Location(string inNm, int[] inCoords)
        {
            nm = inNm;
            coords = inCoords;
            breweryCost = 500;
            ingredients = Ingredient.GetAllIngredients();
        }

        public int GetPrice(string ingNm)
        {
            int idx = ingredients.FindIndex(
                delegate(Ingredient ing)
                {
                    return (ing.nm == ingNm);
                }
            );

            if (idx == -1)
            {
                throw(new IndexOutOfRangeException(
                    "ERROR: Can't find ingredient in market."));
            }

            return ingredients[idx].basePx;
        }

    }

    public abstract class Product
    {
        public Product()
        {
            nm = "EMPTY";
            basePx = 0;
        }

        public Product(string inNm, int inBasePx)
        {
            nm = inNm;
            basePx = inBasePx;
        }

        public string nm { get; protected set; }
        public int basePx { get; protected set; }


    }

    // ----- Ingredient ----------
    public class Ingredient : Product
    {

        public Ingredient()
        {
            nm = "EMPTY";
            basePx = 1;
        }

        public Ingredient(string inNm, int inBasePx)
            : base(inNm, inBasePx)
        {
        }

        public Ingredient(string ingredient)
        {
            nm = ingredient;

            switch (ingredient)
            {
                case "water":
                    basePx = 1;
                    break;
                case "malt: us barley":
                    basePx = 20;
                    break;
                case "hops: cascade":
                    basePx = 50;
                    break;
                case "yeast: american lager":
                    basePx = 20;
                    break;
                default:
                    nm = "EMPTY";
                    Console.WriteLine("Error in Ingredient({0}).", ingredient);
                    break;
            }
        }

        public static int nIngredients { get {return 4;}}

        public static List<Ingredient> GetAllIngredients()
        {
            List<Ingredient> list = new List<Ingredient>();
            list.Add(new Ingredient("water"));
            list.Add(new Ingredient("malt: us barley"));
            list.Add(new Ingredient("hops: cascade"));
            list.Add(new Ingredient("yeast: american lager"));

            return list;
        }

    }

    // ----- Brew ----------
    public class Brew : Product
    {
        public Brew()
        {
            nm = "EMPTY";
            basePx = 1;
        }

        public Brew(string inNm, int inBasePx)
            : base (inNm, inBasePx)
        {
        }

        public Brew(string brew)
        {
            nm = brew;

            switch (brew)
            {
                case "american lager":
                    basePx = 100;
                    break;

                default:
                    nm = "EMPTY";
                    basePx = 1;
                    break;
            }
        }
    }

    public class Recipe
    {
        // Empty constructor
        public Recipe()
        {
            nm = "EMPTY";
            ingredients = new Ingredient[1];
            ingredients[0] = new Ingredient();
            quantities = new int[] { 1 };
            time = 10;
            output = new Brew();
            brewQty = 1;
        }

        // Detailed constructor
        public Recipe(string inNm, Ingredient[] inIngredients,
            int[] inQuantities, int inTime, Brew inOutput,
            int inBrewQty)
        {
            nm = inNm;
            ingredients = inIngredients;
            quantities = inQuantities;
            time = inTime;
            output = inOutput;
            brewQty = inBrewQty;
        }

        // Brew name constructor
        public Recipe(string recipe)
        {
            switch (recipe)
            {
                case "american lager":
                    nm = recipe;
                    
                    ingredients = new Ingredient[4];

                    ingredients[0] = new Ingredient("water");
                    ingredients[1] = new Ingredient("malt: us barley");
                    ingredients[2] = new Ingredient("hops: cascade");
                    ingredients[3] = new Ingredient("yeast: american lager");

                    quantities = new int[4] { 20, 10, 2, 1 };

                    time = 10;
                    output = new Brew("american lager");
                    brewQty = 15;

                    break;

                default:
                    Console.WriteLine("Error in Recipe({0}).", recipe);
                    break;
            }
        }

        public string nm { get; private set; }
        public Ingredient[] ingredients { get; private set; }
        public int[] quantities { get; private set; }
        public int time { get; private set; }
        public Brew output { get; private set; }
        public int brewQty { get; private set; }
    }

    public class FermVessel
    {
        public FermVessel()
        {
            maxSpace = 20;
        }

        public int maxSpace { get; private set; }
        public Brew storedBrew { get; private set; }
        public int timeRemaining { get; private set; }

        
        // Advance the timer
        public void Age(int n)
        {
            timeRemaining = Math.Max(0, timeRemaining - n);
        }

        // Is the vessel empty?
        public bool isEmpty()
        {
            if (storedBrew == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Remove all contents
        public void Empty()
        {
            storedBrew = null;
        }
        
        // Add a brew to the vessel
        // OUTPUT = true if added, false if not.
        public bool Add(Brew inBrew, int inQty, int inTime)
        {
            // Is the vessel empty?
            if (isEmpty())
            {
                // Is the space adequate?
                if (maxSpace >= inQty)
                {
                    // Add the brew to the vessel
                    storedBrew = inBrew;
                    timeRemaining = inTime;
                    return true;
                }
                else
                {
                    // not enough space
                    return false;
                }
            }
            else
            {
                // not empty
                return false;
            }
        }
    }

    // ----- Class: Brewery ----------
    public class Brewery
    {
        public Brewery()
        {
        }

        public Brewery(string inOwner)
        {
            owner = inOwner;

            ingBag = new List<Ingredient>();
            ingQty = new List<int>();

            brewBag = new List<Brew>();
            brewQty = new List<int>();

            fvBag = new List<FermVessel>();
        }

        public List<Ingredient> ingBag { get; private set; }
        public List<int> ingQty { get; private set; }

        public List<Brew> brewBag { get; private set; }
        public List<int> brewQty { get; private set; }

        public List<FermVessel> fvBag { get; private set; }

        public string owner { get; private set; }

        // Add Ingredient to storage
        public bool Add(Ingredient addIngredient, int addQty)
        {
            // Check if some of the ingredient is already present
            int idx = ingBag.FindIndex(
                delegate(Ingredient ing)
                {
                    return (ing.nm == addIngredient.nm &&
                        ing.basePx == addIngredient.basePx);
                }
            );

            if (idx == -1)
            {
                // add new element
                ingBag.Add(addIngredient);
                ingQty.Add(addQty);
            }
            else
            {
                // add to existing element
                ingQty[idx] += addQty;
            }
            
            return true;
        }

        //Remove Ingredient from storage
        public bool Remove(Ingredient remIngredient, int remQty)
        {
            int idx = ingBag.FindIndex(
                delegate(Ingredient ing)
                {
                    return (ing.nm == remIngredient.nm &&
                        ing.basePx == remIngredient.basePx);
                }
            );

            // If some is present and there is enough qty
            if ((idx != -1) && (ingQty[idx] >= remQty))
            {
                // remove remQty
                ingQty[idx] -= remQty;

                // remove entries if new qty is zero
                if (ingQty[idx] == 0)
                {
                    ingBag.RemoveAt(idx);
                    ingQty.RemoveAt(idx);
                }

                // return true
                return true;
            }
            else
            {
                return false;
            }
        }

        // Add Brew to storage
        public bool Add(Brew addBrew, int addQty)
        {
            // Check if some of the ingredient is already present
            int idx = brewBag.FindIndex(
                delegate(Brew brew)
                {
                    return (brew.nm == addBrew.nm &&
                        brew.basePx == addBrew.basePx);
                }
            );

            if (idx == -1)
            {
                // add new element
                brewBag.Add(addBrew);
                brewQty.Add(addQty);
            }
            else
            {
                // add to existing element
                brewQty[idx] += addQty;
            }

            return true;
        }

        // Remove Brew from storage
        public bool Remove(Brew remBrew, int remQty)
        {
            int idx = brewBag.FindIndex(
                delegate(Brew brew)
                {
                    return (brew.nm == remBrew.nm &&
                        brew.basePx == remBrew.basePx);
                }
            );

            // If some is present and there is enough qty
            if ((idx != -1) && (brewQty[idx] >= remQty))
            {
                // remove remQty
                brewQty[idx] -= remQty;

                // remove entries if new qty is zero
                if (brewQty[idx] == 0)
                {
                    brewBag.RemoveAt(idx);
                    brewQty.RemoveAt(idx);
                }

                // return true
                return true;
            }
            else
            {
                return false;
            }
        }

        // Add FermVessel to Brewery
        public bool Add(FermVessel addFermVessel)
        {

            fvBag.Add(addFermVessel);
            return true;

        }

        // Remove FermVessel from Brewery
        public bool RemoveFermVessel(int remIdx)
        {
            fvBag.RemoveAt(remIdx);
            return true;
        }

        // Age each FermVessel in the brewery
        public void AgeAll(int t)
        {
            for (int i = 0; i < fvBag.Count; i++)
            {
                fvBag[i].Age(t);
            }
        }

    }

    public class Company
    {

        public Company(string inNm, int inCash)
        {
            nm = inNm;
            cash = inCash;

            recipes = new List<Recipe>();
            breweries = new List<Brewery>();
            breweryLocations = new List<Location>();
        }

        public string nm { get; private set; }
        public int cash { get; private set; }
        public List<Recipe> recipes { get; private set; }
        public List<Brewery> breweries { get; private set; }
        public List<Location> breweryLocations { get; private set; }

        public bool Add(Brewery inBrewery, Location inLocation)
        {
            // check for existing brewery in same location
            if (breweryLocations.Contains(inLocation))
            {
                return false;
            }
            else
            {
                // add brewery
                breweries.Add(inBrewery);
                breweryLocations.Add(inLocation);
                return true;
            }
        }

        public bool Add(Recipe inRecipe)
        {
            recipes.Add(inRecipe);
            return true;
        }

        public bool Charge(int cost)  // TODO: needs tests
        {
            if (cash >= cost)
            {
                cash -= cost;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
