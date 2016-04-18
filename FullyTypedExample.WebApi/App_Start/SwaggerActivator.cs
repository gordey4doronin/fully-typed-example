// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwaggerActivator.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(FullyTypedExample.WebApi.SwaggerConfig), "Register")]