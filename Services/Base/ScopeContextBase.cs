using HappyFarm.Data.Models;

namespace HappyFarm.Services.Base
{
    public abstract class ScopeContextBase
    {
        private IServiceProvider _provider;

        public ScopeContextBase(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected void ScopeContext(Action<HappyFarmContext> func)
        {
            using (var scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HappyFarmContext>();
                func.Invoke(context);
                context.Dispose();
            }
        }
    }
}
