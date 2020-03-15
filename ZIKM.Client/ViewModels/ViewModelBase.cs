using ReactiveUI;
using ZIKM.Client.Infrastructure.Interfaces;

namespace ZIKM.Client.ViewModels {
    public class ViewModelBase : ReactiveObject {
        private static IProvider provider;

        public IProvider Provider { get => provider; protected set => this.RaiseAndSetIfChanged(ref provider, value); }
    }
}
