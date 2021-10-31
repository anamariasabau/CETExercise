using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CETAPI.Enums;

namespace CETAPI.Model
{
    public class OrderActionModel
    {
        [Required]
        public long OrderId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public Double Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [EnumDataType(typeof(Side))]
        public Side Side { get; set; }
        [Required]
        [EnumDataType(typeof(ActionType))]
        public ActionType Action { get; set; }


        public OrderActionModel()
        {
            OrderId = this.OrderId;
            Timestamp = this.Timestamp;
            Price = this.Price;
            Quantity = this.Quantity;
            Side = this.Side;
            Action = this.Action;
        }
        public OrderActionModel(long _orderId,DateTime _timestamp,Double _price,int _quantity,Enums.Side _side,Enums.ActionType _action)
        {
            OrderId = _orderId;
            Timestamp = _timestamp;
            Price = _price;
            Quantity = _quantity;
            Side = _side;
            Action = _action;
        }

    }


    public class OrderActionModelComparer : IEqualityComparer<OrderActionModel>
    {
        public bool Equals(OrderActionModel x, OrderActionModel y)
        {
            if (x == null && y == null)
                return true;

            return (x.OrderId == y.OrderId && x.Price == y.Price && x.Quantity == y.Quantity && x.Side == y.Side);
        }

        public int GetHashCode(OrderActionModel model)
        {
            var hash = new System.HashCode();

            hash.Add(model.OrderId);
            hash.Add(model.Price);
            hash.Add(model.Quantity);
            hash.Add(model.Side);
            return hash.ToHashCode();
        }
    }
}
