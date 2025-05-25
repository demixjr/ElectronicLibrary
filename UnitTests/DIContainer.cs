using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using BLL.IServices;
using BLL.Services;
using DAL;
using DAL.Models;
using Ninject;
using NSubstitute;

namespace ElectronicLibrary
{
    public class DIContainer
    {
        private IKernel _kernel;
        private IFixture _fixture { get; }

        public DIContainer()
        {
            _kernel = new StandardKernel();
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            var mapperMock = Substitute.For<IMapper>();

            var unitOfWorkMock = Substitute.For<IUnitOfWork>();
            var bookRepositoryMock = Substitute.For<IRepository<Book>>();
            var orderRepositoryMock = Substitute.For<IRepository<Order>>();
            var userRepositoryMock = Substitute.For<IRepository<User>>();

            unitOfWorkMock.GetRepository<Book>().Returns(bookRepositoryMock);
            unitOfWorkMock.GetRepository<Order>().Returns(orderRepositoryMock);
            unitOfWorkMock.GetRepository<User>().Returns(userRepositoryMock);

            _kernel.Bind<IMapper>().ToConstant(mapperMock);
            _kernel.Bind<IUnitOfWork>().ToConstant(unitOfWorkMock);

            _kernel.Bind<IRepository<Book>>().ToConstant(bookRepositoryMock);
            _kernel.Bind<IBookService>().To<BookService>();

            _kernel.Bind<IRepository<Order>>().ToConstant(orderRepositoryMock);
            _kernel.Bind<IOrderService>().To<OrderService>();

            _kernel.Bind<IRepository<User>>().ToConstant(userRepositoryMock);
            _kernel.Bind<IUserService>().To<UserService>();


            _kernel.Bind<IFixture>().ToConstant(_fixture);
        }


        public T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public IFixture GetFixture()
        {
            return _fixture;
        }
    }
}
