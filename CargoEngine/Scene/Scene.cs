using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoEngine.Scene {
    public class Scene {
        public SceneNode RootNode {
            get; private set;
        }

        public Scene() {
            RootNode = new SceneNode();
        }
    }
}
