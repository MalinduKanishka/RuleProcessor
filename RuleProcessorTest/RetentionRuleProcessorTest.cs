using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using RuleProcessor.Application.Abstractions;
using RuleProcessor.Application.Models;
using RuleProcessor.Application.Repository;
using RuleProcessor.RetentionApplication;
using RuleProcessorTest.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RuleProcessorTest
{
    public class RetentionRuleProcessorTest
    {
        private readonly IConfiguration _configuration;
        private readonly ICacheLogger<RetentionRuleProcessor> _logger;
        private readonly Mock<RetentionRuleProcessor> _retentionRuleProcessor;
        private readonly Mock<ProjectRepository> _projectRepository;
        private readonly Mock<EnvironmentRepository> _enviornmentRepository;
        private readonly Mock<ReleaseRepository> _releaseRepository;
        private readonly Mock<DeploymentRepository> _deploymentRepository;
        private readonly ICacheLogger<ProjectRepository> _loggerProject;
        private readonly ICacheLogger<EnvironmentRepository> _loggerEnvironment;
        private readonly ICacheLogger<ReleaseRepository> _loggerRelease;
        private readonly ICacheLogger<DeploymentRepository> _loggerDeployment;


        public RetentionRuleProcessorTest()
        {
            _configuration = TestHelper.GetIConfiguration();
            _logger = Substitute.For<ILogger<RetentionRuleProcessor>>().WithCache();
            _logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            _loggerProject = Substitute.For<ILogger<ProjectRepository>>().WithCache();
            _loggerProject.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            _loggerEnvironment = Substitute.For<ILogger<EnvironmentRepository>>().WithCache();
            _loggerEnvironment.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            _loggerRelease = Substitute.For<ILogger<ReleaseRepository>>().WithCache();
            _loggerRelease.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            _loggerDeployment = Substitute.For<ILogger<DeploymentRepository>>().WithCache();
            _loggerDeployment.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            _projectRepository = new Mock<ProjectRepository>(_configuration, _loggerProject);
            _enviornmentRepository = new Mock<EnvironmentRepository>(_configuration, _loggerEnvironment);
            _releaseRepository = new Mock<ReleaseRepository>(_configuration, _loggerRelease);
            _deploymentRepository = new Mock<DeploymentRepository>(_configuration, _loggerDeployment);
            _retentionRuleProcessor = new Mock<RetentionRuleProcessor>(_projectRepository.Object, _enviornmentRepository.Object,
                                                            _releaseRepository.Object, _deploymentRepository.Object, _configuration, _logger);
        }


        [Fact(DisplayName = "A Start Background Process")]
        public async Task AStartBackgroundProcess()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            await _retentionRuleProcessor.Object.StartAsync(cancellationTokenSource.Token);

        }

        [Fact(DisplayName = "B Stop Background Process")]
        public async Task BStopBackgroundProcess()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            await _retentionRuleProcessor.Object.StartAsync(CancellationToken.None);

            await _retentionRuleProcessor.Object.StopAsync(cancellationTokenSource.Token);
        }

        [Fact(DisplayName = "C Test RetentionRuleProcessor")]
        public async Task CTestRetentionRuleProcessor()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            await _retentionRuleProcessor.Object.StartAsync(CancellationToken.None);
            int numberOfRetentions = _configuration.GetValue<int>("NumberOfRetentions");

            var releases = _retentionRuleProcessor.Object.ProcessRule(numberOfRetentions);

            releases.Result.Should().HaveCount(4);

            await _retentionRuleProcessor.Object.StopAsync(cancellationTokenSource.Token);
        }

        [Fact(DisplayName = "D Test RetentionRuleProcessor -Keep 2 Releases")]
        public async Task DTestRetentionRuleProcessorKeep2Releases()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            await _retentionRuleProcessor.Object.StartAsync(CancellationToken.None);
            int numberOfRetentions = 2;

            var releases = _retentionRuleProcessor.Object.ProcessRule(numberOfRetentions);

            releases.Result.Should().HaveCount(6);

            await _retentionRuleProcessor.Object.StopAsync(cancellationTokenSource.Token);
        }

        [Fact(DisplayName = "E Validate Deployment Repository FindAll")]
        public void EValidateDeploymentRepositoryFindAll()
        {
            _deploymentRepository.Object.LoadData();

            var deployments = _deploymentRepository.Object.FindAll();

            deployments.Should().HaveCount(10);
        }

        [Fact(DisplayName = "F Validate Releases Repository FindAll")]
        public void FValidateReleasesRepositoryFindAll()
        {
            _releaseRepository.Object.LoadData();

            var releases = _releaseRepository.Object.FindAll();

            releases.Should().HaveCount(8);
        }

        [Fact(DisplayName = "G Validate Environment Repository FindAll")]
        public void GValidateEnvironmentsRepositoryFindAll()
        {
            _enviornmentRepository.Object.LoadData();

            var environments = _enviornmentRepository.Object.FindAll();

            environments.Should().HaveCount(2);
        }

        [Fact(DisplayName = "H Validate Project Repository FindAll")]
        public void HValidateProjectRepositoryFindAll()
        {
            _projectRepository.Object.LoadData();

            var projects = _projectRepository.Object.FindAll();

            projects.Should().HaveCount(2);
        }
    }
}