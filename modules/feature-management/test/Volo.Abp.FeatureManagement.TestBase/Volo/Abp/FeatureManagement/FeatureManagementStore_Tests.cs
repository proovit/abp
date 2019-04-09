﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Features;
using Volo.Abp.Modularity;
using Xunit;

namespace Volo.Abp.FeatureManagement
{
    public abstract class FeatureManagementStore_Tests<TStartupModule> : FeatureManagementTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private IFeatureManagementStore FeatureManagementStore { get; set; }
        private IFeatureValueRepository FeatureValueRepository { get; set; }

        protected FeatureManagementStore_Tests()
        {
            FeatureManagementStore = GetRequiredService<IFeatureManagementStore>();
            FeatureValueRepository = GetRequiredService<IFeatureValueRepository>();
        }

        [Fact]
        public async Task GetOrNullAsync()
        {
            // Act
            (await FeatureManagementStore.GetOrNullAsync(Guid.NewGuid().ToString("N"),
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).ShouldBeNull();

            (await FeatureManagementStore.GetOrNullAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Get_Null_Where_Feature_Deleted()
        {
            // Arrange
            (await FeatureManagementStore.GetOrNullAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).ShouldNotBeNull();

            // Act
            await FeatureManagementStore.DeleteAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"));

            // Assert
            (await FeatureManagementStore.GetOrNullAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).ShouldBeNull();
        }

        [Fact]
        public async Task SetAsync()
        {
            // Arrange
            (await FeatureValueRepository.FindAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).Value.ShouldBe(true.ToString().ToLowerInvariant());

            // Act
            await FeatureManagementStore.SetAsync(TestFeatureDefinitionProvider.SocialLogins,
                false.ToString().ToUpperInvariant(),
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"));

            // Assert
            (await FeatureValueRepository.FindAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).Value.ShouldBe(false.ToString().ToUpperInvariant());
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Arrange
            (await FeatureValueRepository.FindAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).ShouldNotBeNull();

            // Act
            await FeatureManagementStore.DeleteAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"));


            // Assert
            (await FeatureValueRepository.FindAsync(TestFeatureDefinitionProvider.SocialLogins,
                EditionFeatureValueProvider.ProviderName,
                TestEditionIds.Regular.ToString("N"))).ShouldBeNull();


        }
    }
}