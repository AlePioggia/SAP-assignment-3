using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.Fluent;
using Xunit;

using static ArchUnitNET.Fluent.ArchRuleDefinition;
using ArchUnitNET.xUnit;

namespace UserService.UserService.Tests
{
    public class ArchUnitTest
    {
        private static readonly Architecture Architecture = new ArchLoader().LoadAssemblies(
            typeof(domain.entities.User).Assembly,
            typeof(application.IUserService).Assembly,
            typeof(controller.UserController).Assembly,
            typeof(infrastructure.UserRepository).Assembly
        ).Build();

        private readonly IObjectProvider<IType> DomainLayer =
            Types().That().ResideInNamespace("UserService.domain").As("Domain Layer");

        private readonly IObjectProvider<IType> ApplicationLayer =
            Types().That().ResideInNamespace("UserService.application").As("Application Layer");

        private readonly IObjectProvider<IType> ControllerLayer =
            Types().That().ResideInNamespace("UserService.controller").As("Controller Layer");

        private readonly IObjectProvider<IType> InfrastructureLayer =
            Types().That().ResideInNamespace("UserService.infrastructure").As("Infrastructure Layer");

        [Fact]
        public void DomainLayer_ShouldNotDependOnOtherLayers()
        {
            IArchRule domainLayerShouldNotDependOnOtherLayers =
                Types().That().Are(DomainLayer).Should()
                .NotDependOnAny(ApplicationLayer).AndShould()
                .NotDependOnAny(InfrastructureLayer)
                .Because("the domain layer should not depend on other layers");

            domainLayerShouldNotDependOnOtherLayers.Check(Architecture);
        }

        [Fact]
        public void ApplicationLayer_ShouldDependOnlyOnDomain()
        {
            IArchRule applicationLayerShouldDependOnlyOnDomain =
                Types().That().Are(ApplicationLayer).Should()
                .OnlyDependOn(DomainLayer)
                .Because("the application layer should only interact with the domain layer");

            applicationLayerShouldDependOnlyOnDomain.Check(Architecture);
        }

        [Fact]
        public void ControllerLayer_ShouldDependOnlyOnApplicationAndDomain()
        {
            IArchRule controllerLayerShouldNotDependOnOtherLayers =
                Types().That().Are(ControllerLayer).Should()
                .NotDependOnAny(InfrastructureLayer)
                .Because("the controller layer should depend only on application and domain layers");

            controllerLayerShouldNotDependOnOtherLayers.Check(Architecture);
        }

        [Fact]
        public void ApplicationLayer_ShouldUseInterfacesToAccessInfrastructure()
        {
            IArchRule rule =
                Types().That().Are(ApplicationLayer).Should()
                .OnlyDependOnTypesThat().Are(Interfaces())
                .Because("the application layer should interact with the infrastructure layer through interfaces");

            rule.Check(Architecture);
        }
    }
}
