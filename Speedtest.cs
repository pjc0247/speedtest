using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace speedtest
{
    class Testcase
    {
        protected virtual int iteration => 2000000;

        public double timeA { get; private set; }
        public double timeB { get; private set; }

        private string nameA, nameB;

        private Action<object> actionA, actionB;

        protected virtual object Prepare()
        {
            return null;
        }
        protected void A(Expression<Action<object>> func)
        {
            if (func.Body is MethodCallExpression mcall)
                nameA = mcall.Method.Name;

            actionA = func.Compile();
        }
        protected void B(Expression<Action<object>> func)
        {
            if (func.Body is MethodCallExpression mcall)
                nameB = mcall.Method.Name;

            actionB = func.Compile();
        }

        public void Execute()
        {
            var p = Prepare();
            GC.Collect();

            var st = DateTime.Now;
            actionA.Invoke(p);
            timeA = (DateTime.Now - st).TotalMilliseconds;

            p = Prepare();
            GC.Collect();

            st = DateTime.Now;
            actionB.Invoke(p);
            timeB = (DateTime.Now - st).TotalMilliseconds;
        }

        public override string ToString()
        {
            return $"{nameA} : {timeA} / {nameB} : {timeB}";
        }
    }

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

    class Program
    {
        static void Main(string[] args)
        {
            var c = new ExchangeOrClear();

            c.Execute();

            Console.WriteLine(c.ToString());
        }
    }
}
