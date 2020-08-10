//using System;

//using MediatR;

//namespace NatashaPad.MvvmServices.ViewRequests
//{
//    public class ShowViewRequest : IRequest
//    {
//        public ShowViewRequest(object vm, Type type)
//        {
//            ViewModel = vm;
//            Type = type;
//        }

//        public object ViewModel { get; }
//        public Type Type { get; }

//        public static ShowViewRequest Create<TViewModel>(TViewModel vm)
//        {
//            return new ShowViewRequest(vm, typeof(TViewModel));
//        }
//    }
//}
