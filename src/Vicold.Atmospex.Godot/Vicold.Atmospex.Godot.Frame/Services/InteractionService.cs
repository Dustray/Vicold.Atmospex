using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Numerics;
using Vicold.Atmospex.Earth.Events;
using static Godot.XRPositionalTracker;

namespace Vicold.Atmospex.Godot.Frame.Services
{
    public class OrderInfo
    {
        public object UserData { get; set; }
        public Action BlockCompeletedAction { get; set; }
    }

    public class OrderClubMember
    {
        public OrderClubMember(string order)
        {
            Order = order;
        }

        public string Order { get; }
        public bool IsMapBlock { get; set; }
        public Action<OrderInfo> BackAction { get; set; }
    }

    public class OrderClub
    {
        private ConcurrentDictionary<string, OrderClubMember> _orderList;

        internal OrderClub()
        {
            _orderList = new ConcurrentDictionary<string, OrderClubMember>();
        }

        public void Register(string order, Action<OrderInfo> action)
        {
            RegisterMapBlock(order, action, false);
        }

        public void RegisterMapBlock(string order, Action<OrderInfo> action)
        {
            RegisterMapBlock(order, action, true);
        }

        private void RegisterMapBlock(string order, Action<OrderInfo> action, bool isMapBlock)
        {
            if (_orderList.TryGetValue(order, out _))
            {
                throw new Exception("Repeated action order.");
            }

            if (action == null)
            {
                throw new Exception("Null action.");
            }

            _orderList[order] = new OrderClubMember(order)
            {
                IsMapBlock = isMapBlock,
                BackAction = action,
            };
        }

        public bool Execute(string order, object context, bool throwExIfNotFound = false)
        {
            if (_orderList.TryGetValue(order, out var action))
            {
                var orderInfo = new OrderInfo() { UserData = context };
                if (action.IsMapBlock)
                {
                    orderInfo.BlockCompeletedAction = OnBlockCompeleted;
                    SendMapBlockOrder(true);
                }
                action.BackAction.Invoke(orderInfo);
                return true;
            }
            else
            {
                if (throwExIfNotFound)
                {
                    throw new Exception("Order not found.");
                }
                else
                {
                    return false;
                }
            }
        }

        private void OnBlockCompeleted()
        {
            SendMapBlockOrder(false);
        }

        private void SendMapBlockOrder(bool isBlock)
        {
            if (_orderList.TryGetValue("OnMapBlock", out var action))
            {
                var orderInfo = new OrderInfo() { UserData = isBlock };
                action.BackAction.Invoke(orderInfo);
            }
        }
    }

    public class InteractionService: IInteractionService
    {
        public InteractionService()
        {
            Order = new OrderClub();
        }

        public event MouseMoveEventHandler OnMouseMove;

        public OrderClub Order { get; }

        public void MouseMove(Vector2 position)
        {
            OnMouseMove?.Invoke(this, new MouseMoveEventArgs(position, position));
        }
    }
}
