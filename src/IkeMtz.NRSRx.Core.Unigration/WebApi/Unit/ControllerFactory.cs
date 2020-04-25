using System;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi.Unit
{
  public static class ControllerFactory<TController> where TController : ControllerBase
  {
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
