using System;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi.Unit
{
  public static class ControllerFactory<Controller> where Controller : ControllerBase
  {
    public static Controller Create(params object[] args)
    {
      Controller controller;
      try
      {
        controller = (Controller)Activator.CreateInstance(typeof(Controller), args);
      }
      catch (MissingMethodException e)
      {
        var msg = $"Developer exception: No constructor for type: {typeof(Controller).Name} found matching the specified arguments.";
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
