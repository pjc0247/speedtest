# speedtest
Tiny framework for measure execution time between methods.

Example
----
```cs
class ExchangeOrClear : Testcase
{
    public ExchangeOrClear()
    {
        A(_ => Exchange((Dictionary<string, string>)_));
        B(_ => Clear((Dictionary<string, string>)_));
    }

    protected override object Prepare()
    {
        var bigDict = new Dictionary<string, string>();
        for (int i = 0; i < 200000; i++)
            bigDict.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        return bigDict;
    }

    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private void Exchange(Dictionary<string, string> dict)
    {
        for (int i = 0; i < iteration; i++)
        {
            var b = dict;
            dict = new Dictionary<string, string>();
        }
    }
    [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
    private void Clear(Dictionary<string, string> dict)
    {
        for (int i = 0; i < iteration; i++)
        {
            var b = new Dictionary<string, string>(dict);
            dict.Clear();
        }
    }
}
```
```
Exchange : 48.1004 / Clear : 166.9437
```
