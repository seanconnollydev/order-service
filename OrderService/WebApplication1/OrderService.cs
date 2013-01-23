﻿using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceModel;
using System.Web;

namespace OrderService
{
    public class OrderService : RestServiceBase<Order>
    {
        private const string ORDER_QUEUE_PATH = @".\private$\Orders"; // TODO: What is the .\ doing?

        public override object OnGet(Order request)
        {
            throw new NotImplementedException();
        }

        public override object OnPost(Order request)
        {
            MessageQueue orderQueue = new MessageQueue(ORDER_QUEUE_PATH, QueueAccessMode.Send);

            if (!MessageQueue.Exists(orderQueue.Path))
            {
                MessageQueue.Create(orderQueue.Path);
            }

            try
            {
                orderQueue.Send(request);
                return "OK";
            }
            catch (Exception e)
            {
                // TODO: Log this somewhere

                throw e; // ServiceStack will automatically interpret an exception as an HTTP error: https://github.com/ServiceStack/ServiceStack/wiki/Validation
            }
            finally
            {
                orderQueue.Close();
            }
        }
    }
}