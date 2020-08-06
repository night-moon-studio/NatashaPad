using System.Windows;

using MediatR;

namespace NatashaPad.MvvmServices.ViewRequests
{
    public class ShowViewRequestHandler : RequestHandler<ShowViewRequest>
    {
        private readonly IViewInstanceLocator locator;

        public ShowViewRequestHandler(IViewInstanceLocator locator)
        {
            this.locator = locator;
        }

        protected override void Handle(ShowViewRequest request)
        {
            var window = new Window();
            window.Content = locator.GetView(request.Type);
            window.DataContext = request.ViewModel;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ShowDialog();

            /* 
             * TODO: 可选的解法：
             * 在资源文件中定义View和ViewModel的映射关系
             * 预先设计好默认window
             * window中的内容元素的内容绑定到DataContext
             * 注册时，提供name，窗口尺寸等其它信息
             * 这样，就不需要在这个方法里硬编码，把扩展性开放出去
             */
        }
    }
}