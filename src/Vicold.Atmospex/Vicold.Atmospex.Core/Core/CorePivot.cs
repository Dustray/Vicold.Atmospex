using Vicold.Atmospex.DataCenter;
using Vicold.Atmospex.DataHub;
using Vicold.Atmospex.Earth;
using Vicold.Atmospex.Earth.Projection;
using Vicold.Atmospex.Core.Bus;
using Vicold.Atmospex.Core.Config;
using Vicold.Atmospex.Core.Entities;
using Vicold.Atmospex.Core.LayerService;
using Vicold.Atmospex.GisMap;
using Vicold.Atmospex.Interaction;
using Vicold.Atmospex.Layer;
using Vicold.Atmospex.Vision.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Atmospex.Core.Core
{
    internal class CorePivot
    {
        public CorePivot()
        {
        }

        public void Boot(BootConfig bootConfig)
        {
            string workSpace;
#if DEBUG
            workSpace = bootConfig.WorkSpaceDebug;
#else
            workSpace =bootConfig.WorkSpace);
#endif
            SetEnvironment(workSpace);

            var bus = GlobalBus.Current;

            var configuration = bus.RegisterTransport<IGlobalConfiguration, GlobalConfiguration>();
            configuration.Init(bootConfig);

            DataCenter = new DataCenterGate(bus, workSpace);

            Vision = new VisionGate(bus);
            Earth = new EarthGate(bus);
            InitTransports();
            Map = new MapGate(bus);
            Interaction = new InteractionGate(bus);
            OnEngineLaunched?.Invoke();
            DataHub = new DataHubGate(bus);
        }

        private void InitTransports()
        {
            var earthTransport = GlobalBus.Current.GetTransport<IEarthTransport>();
            var layerManager = GlobalBus.Current.RegisterTransport<ILayerManager, LayerManager>();
            layerManager.SetProjection(earthTransport.CurrentProjection);
        }

        public Action OnEngineLaunched { get; set; }

        public VisionGate Vision { get; private set; }

        public EarthGate Earth { get; private set; }

        /// <summary>
        /// 交互
        /// </summary>
        public InteractionGate Interaction { get; private set; }

        /// <summary>
        /// 地图
        /// </summary>
        public MapGate Map { get; private set; }

        /// <summary>
        /// 数据转发集合、数据绑定
        /// </summary>
        public DataHubGate DataHub { get; set; }

        /// <summary>
        /// 产品数据中心
        /// </summary>
        public DataCenterGate DataCenter { get; private set; }

        private void SetEnvironment(string work)
        {
            // 环境变量注册.
            var defaultEnvs = Environment.GetEnvironmentVariable("PATH");
            var envs = new StringBuilder();
            try
            {
                var runtimePath = Path.Combine(work, "dll");
                envs.Append($";{runtimePath}");
                var runtimes = Directory.GetDirectories(runtimePath);
                foreach (var r in runtimes)
                {
                    envs.Append($";{r}");
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                envs.Append($";{defaultEnvs}");
                Environment.SetEnvironmentVariable("PATH", envs.ToString().TrimStart(';'));
            }

        }
    }
}
