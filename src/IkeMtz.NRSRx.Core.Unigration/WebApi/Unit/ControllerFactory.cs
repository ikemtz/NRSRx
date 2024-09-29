using System;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi.Unit
{
  /// <summary>
  /// Provides a factory for creating controller instances for unit testing.
  /// </summary>
  /// <typeparam name="TController">The type of the controller.</typeparam>
  public static class ControllerFactory<TController> where TController : ControllerBase
  {
    /// <summary>
    /// Creates an instance of the specified controller type with the provided arguments.
    /// </summary>
    /// <param name="args">The arguments to pass to the controller's constructor.</param>
    /// <returns>An instance of the specified controller type.</returns>
    /// <exception cref="ArgumentException">Thrown when no matching constructor is found for the specified arguments.</exception>
    public static TController Create(params object[] args)
    {
      TController controller;
      try
      {
        controller = (TController)Activator.CreateInstance(typeof(TController), args);
      }
      catch (MissingMethodException e)
      {
        var msg = $"Developer exception: No constructor for type: {typeof(TController).Name} found matching the specified arguments.";
        throw new ArgumentException(msg, e);
      }
      var objectValidator = new TestingObjectValidator
      {
        Controller = controller,
      };
      controller.ObjectValidator = objectValidator;
      return controller;
    }
  }
}
