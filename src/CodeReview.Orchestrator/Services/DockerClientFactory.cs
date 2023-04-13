using System;
using System.Security.Cryptography.X509Certificates;
using Docker.DotNet;
using GodelTech.CodeReview.Orchestrator.Model;
using BasicAuthCredentials = Docker.DotNet.BasicAuth.BasicAuthCredentials;
using CertificateCredentials = Docker.DotNet.X509.CertificateCredentials;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerClientFactory : IDockerClientFactory
    {
        private readonly IDockerEngineContext _engineContext;

        public DockerClientFactory(IDockerEngineContext engineContext)
        {
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
        }

        public IDockerClient Create()
        {
            switch (_engineContext.Engine.AuthType)
            {
                case AuthType.Basic:
                {
                    return new DockerClientConfiguration(
                            new Uri(_engineContext.Engine.Url),
                            new BasicAuthCredentials(
                                _engineContext.Engine.BasicAuthCredentials.Username,
                                _engineContext.Engine.BasicAuthCredentials.Password))
                        .CreateClient();
                }
                case AuthType.X509:
                {
                    return new DockerClientConfiguration(
                            new Uri(_engineContext.Engine.Url),
                            new CertificateCredentials(new X509Certificate2(
                                _engineContext.Engine.CertificateCredentials.FileName,
                                _engineContext.Engine.CertificateCredentials.Password)))
                        .CreateClient();
                }
                default:
                {
                    return new DockerClientConfiguration(
                            new Uri(_engineContext.Engine.Url))
                        .CreateClient();
                }
            }
        }
    }
}