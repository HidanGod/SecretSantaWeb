using System;
using System.Linq;

namespace SecretSantaWeb
{
    public static class GeneratorRandomSant
    {
        public static int[] GenerateRandomNumsSant(int count)
        {
            var generateRandomNumsSant = new int[count];
            if (count == 1) return generateRandomNumsSant;
            var permutation = Enumerable.Range(0, count).ToList();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                var num = random.Next(permutation.Count);
                while (permutation[num] == i)
                {
                    num = random.Next(permutation.Count);
                }
                generateRandomNumsSant[i] = permutation[num];
                permutation.Remove(num);
            }
            return generateRandomNumsSant;
        }
    }
}
