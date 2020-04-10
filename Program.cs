using System;
using System.Collections.Generic;
using System.Linq;

namespace GangeltSimulation
{
    class Program
    {
        static void Main(string[] args)
        {            
            List<double> fractionHouseHolds = new List<double>();
            double responseProbabilityIndividual = 0.66;

            for(int i = 0; i < 5000; i++)
            {
                var result = Simulation(responseProbabilityIndividual);

                fractionHouseHolds.Add(result);                
            }

            Console.WriteLine(responseProbabilityIndividual);
            Console.WriteLine("Fraction Households average:" + fractionHouseHolds.Average().ToString("F2"));
            Console.WriteLine("Fraction Households minimum:" + fractionHouseHolds.Min().ToString("F2"));
            Console.WriteLine("Fraction Households maximum:" + fractionHouseHolds.Max().ToString("F2"));
        }

        private static double Simulation(double responseProbabilityOfPerson)
        {
            double responseProbabilityPerson = responseProbabilityOfPerson;
            List<Household> houseHolds = new List<Household>();
            List<Person> people = new List<Person>();
            Dictionary<int, int> householdAmountBySize = new Dictionary<int, int>();
            HashSet<int> houseHoldsWhichRespondedById = new HashSet<int>();
            HashSet<int> peopleWhoRespondedById = new HashSet<int>();

            householdAmountBySize.Add(1, 1109); //Householdsize Distribution taken from 2011 Zensus
            householdAmountBySize.Add(2, 1559); //https://ergebnisse.zensus2011.de/#StaticContent:053700008008,GWZ_4_3_0,m,table
            householdAmountBySize.Add(3, 796);  //not the newest Data but household size was 2.5 then, and ist 2.5 in 2019 according to:
            householdAmountBySize.Add(4, 658);  //https://www.kreis-heinsberg.de/cms/upload/InWIS_Wohnungsmarktstudie_Kreis_Heinsberg_2019.pdf
            householdAmountBySize.Add(5, 197);
            householdAmountBySize.Add(6, 90); //note: this is actually 6+ in the statistics but error here is small (try change size to 7 and it won't change much)

            int houseHoldIncrementer = 0;
            int peopleIncrementer = 0;

            foreach (var kvp in householdAmountBySize)
            {
                int size = kvp.Key;
                int amount = kvp.Value;
                for (int i = 0; i < amount; i++)
                {
                    Household x = new Household(houseHoldIncrementer, size);
                    houseHoldIncrementer++;
                    houseHolds.Add(x);
                }
            }

            foreach (var houseHold in houseHolds)
            {
                for (int i = 0; i < houseHold.Size; i++)
                {
                    Person p = new Person(houseHold.ID, peopleIncrementer);
                    people.Add(p);
                    peopleIncrementer++;
                }
            }

            Random rng = new Random();
            people.Shuffle(rng);

            int amountToPick = (int)(people.Count * responseProbabilityPerson);
            for(int i = 0; i < amountToPick; i++)
            {
                houseHoldsWhichRespondedById.Add(people[i].houseHoldId);
                peopleWhoRespondedById.Add(people[i].ID);
            }
            return houseHoldsWhichRespondedById.Count / (double)houseHolds.Count;
        }        
    }

    public static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list, Random rnd)
        {
            for (var i = list.Count; i > 0; i--)
                list.Swap(0, rnd.Next(0, i));
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    class Household
    {
        public int ID;
        public int Size;

        public Household(int id, int size)
        {
            this.ID = id;
            this.Size = size;
        }

    }

    class Person
    {
        public int houseHoldId;
        public int ID;

        public Person(int houseHoldId, int id)
        {
            this.houseHoldId = houseHoldId;
            this.ID = id;
        }
    }
}
