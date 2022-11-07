namespace shunda.Test;

public class ListUnitTest
{
    [Test]
    public void TestToListCopy()
    {
        IList<int> list1 = new List<int>() {
            1, 2, 3, 4
        };

        IList<int> list2 = list1.ToList();
        
        list2.Add(5);
        
        Console.WriteLine(string.Join(",", list1.Select(e => e.ToString())));
        Console.WriteLine(string.Join(",", list2.Select(e => e.ToString())));
        
        Assert.False(list1 == list2);
    }
}