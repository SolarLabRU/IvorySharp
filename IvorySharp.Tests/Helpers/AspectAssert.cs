﻿using System;
using IvorySharp.Tests.Aspects;
using Xunit;

namespace IvorySharp.Tests.Helpers
{
    public static class AspectAssert
    {
        public static void OnEntryCalled(Type aspectType)
        {
            Assert.True(ObservableBoundaryAspect.HasContext(aspectType, BoundaryType.Entry));
        }

        public static void OnSuccessCalled(Type aspectType)
        {
            Assert.True(ObservableBoundaryAspect.HasContext(aspectType, BoundaryType.Success));
        }

        public static void OnExitCalled(Type aspectType)
        {
            Assert.True(ObservableBoundaryAspect.HasContext(aspectType, BoundaryType.Exit));
        }

        public static void OnExceptionCalled(Type aspectType)
        {
            Assert.True(ObservableBoundaryAspect.HasContext(aspectType, BoundaryType.Exception));
        }
    }
}