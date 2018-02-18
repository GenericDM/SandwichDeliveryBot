using SandwichDeliveryBot3.CustomClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot.Databases;

namespace SandwichDeliveryBot3.Services
{
    public class QueueService
    {
        public LinkedList<Sandwich> CurrentQueue { get; set; } = new LinkedList<Sandwich>();
        private SandwichDatabase db;

        public QueueService(IServiceProvider p)
        {
            db = p.GetService<SandwichDatabase>();
            GenerateQueue();
        }

        public void GenerateQueue()
        {
            var a = db.Sandwiches.ToArray();
            var sorted = a.OrderBy(x => x.date);
            foreach (var s in sorted)
            {
                CurrentQueue.AddLast(s);
            }
        }

        public void JumpOrder(Sandwich o)
        {
            if (CurrentQueue.Contains(o))
            {
                CurrentQueue.Remove(o);
                CurrentQueue.AddFirst(o);
            }
        }

        public void AddOrder(Sandwich o)
        {
            CurrentQueue.AddLast(o);
        }

        public void RemoveOrder(Sandwich o)
        {
            if(CurrentQueue.Contains(o))
                CurrentQueue.Remove(o);
        }

        public Sandwich GetNextOrder()
        {
            return CurrentQueue.First.Value;
        }

        public void AcceptOrder() => CurrentQueue.RemoveFirst();
        public void Skip() => CurrentQueue.RemoveFirst();
    }
}
