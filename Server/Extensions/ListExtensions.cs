namespace KnowledgeConquest.Server.Extensions
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = Random.Shared.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static void Ensure<T>(this IList<T> list, int number)
        {
            var initialSize = list.Count;
            while (list.Count < number)
            {
                var element = initialSize == 0 ? default : list[Random.Shared.Next(initialSize)];
                list.Add(element!);
            }
        }
    }
}
