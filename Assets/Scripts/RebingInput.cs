using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace DefaultNamespace {
    [UxmlObject]
    public partial class RebingInput : CustomBinding {
        public enum Device {
            Keyboard,
            Gamepad
        }
        
        [UxmlAttribute]
        public Device device;

        public RebingInput() {
            updateTrigger = BindingUpdateTrigger.OnSourceChanged;
        }

        protected override void OnDataSourceChanged(in DataSourceContextChanged context) {
            var element = context.targetElement;

            object data = element.dataSource;
            if (element.dataSource == null) {
                DataSourceContext parentContext = element.GetHierarchicalDataSourceContext();
                data = parentContext.dataSource;
            }

            if (data != null) {
                if (data is InputAction inputAction) {
                    var binding = GetInputBindingForDevice(device, inputAction);
                    var value = "";
                    if (binding.isComposite)
                        value = binding.name;
                    else
                        value = binding.ToDisplayString();

                    ConverterGroups.TrySetValueGlobal(ref element, context.bindingId, value, out var errorCode);
                }
            }
        }

        private InputBinding GetInputBindingForDevice(Device device, InputAction inputAction) {
            foreach (var binding in inputAction.bindings) {
                if(binding.isComposite) continue;
                if(device == Device.Keyboard && binding.isComposite && binding.path.Contains("Dpad"))
                    return binding;
                if(device == Device.Keyboard && binding.groups.Contains("Keyboard"))
                    return binding;
                if(device == Device.Gamepad && binding.groups.Contains("Gamepad"))
                    return binding;
            }

            return new InputBinding();
        }
    }
}