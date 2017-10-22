using System;
using System.Collections.Generic;
using CargoEngine.Exception;
using SharpDX;
using SharpDX.Direct3D11;

namespace CargoEngine.Parameter
{

    public class ParameterCollection
    {
        protected Dictionary<string, RenderParameter> parameters = new Dictionary<string, RenderParameter>();

        public IReadOnlyDictionary<string, RenderParameter> Parameters {
            get {
                return parameters;
            }
        }

        public void SetParameter(string name, Matrix mat) {
            if (!SetParam(name, mat, RenderParameterType.Matrix)) {
                parameters.Add(name, new MatrixParameter(mat));
            }
        }

        public RenderParameter GetParameter(string name) {
            if (parameters.ContainsKey(name)) {
                return parameters[name];
            }
            return null;
        }

        public Matrix GetMatrixParameter(string name) {
            return (Matrix)GetParam(name, RenderParameterType.Matrix);
        }

        public void SetParameter(string name, Vector2 vec) {
            if (!SetParam(name, vec, RenderParameterType.Vector2)) {
                parameters.Add(name, new Vector2Parameter(vec));
            }
        }
        public Vector2 GetVector2Parameter(string name) {
            return (Vector2)GetParam(name, RenderParameterType.Vector2);
        }

        public void SetParameter(string name, Vector3 vec) {
            if (!SetParam(name, vec, RenderParameterType.Vector3)) {
                parameters.Add(name, new Vector3Parameter(vec));
            }
        }
        public Vector3 GetVector3Parameter(string name) {
            return (Vector3)GetParam(name, RenderParameterType.Vector3);
        }

        public void SetParameter(string name, Vector4 vec) {
            if (!SetParam(name, vec, RenderParameterType.Vector4)) {
                parameters.Add(name, new Vector4Parameter(vec));
            }
        }
        public Vector4 GetVector4Parameter(string name) {
            return (Vector4)GetParam(name, RenderParameterType.Vector4);
        }

        public void SetParameter(string name, ShaderResourceView srv) {
            if (!SetParam(name, srv, RenderParameterType.SRV)) {
                parameters.Add(name, new ShaderResourceParameter(srv));
            }
        }
        public ShaderResourceView GetSRVParameter(string name) {
            return (ShaderResourceView)GetParam(name, RenderParameterType.SRV);
        }

        public void SetParameter(string name, SamplerState sampler) {
            if (!SetParam(name, sampler, RenderParameterType.SamplerState)) {
                parameters.Add(name, new SamplerStateParameter(sampler));
            }
        }
        public SamplerState GetSamplerStateParameter(string name) {
            return (SamplerState)GetParam(name, RenderParameterType.SamplerState);
        }

        public void SetParameter(string name, Texture.Texture texture) {
            if (!SetParam(name, texture, RenderParameterType.Texture)) {
                parameters.Add(name, new TextureParameter(texture));
            }
        }
        public Texture.Texture GetTextureParameter(string name) {
            return (Texture.Texture)GetParam(name, RenderParameterType.Texture);
        }

        protected bool SetParam(string name, object obj, RenderParameterType type) {
            if (parameters.ContainsKey(name)) {
                var param = parameters[name];
                if (param.Type != type) {
                    throw CargoEngineException.Create("Wrong Parametertype expected " + type + " but was " + param.GetType());
                }
                param.Value = obj;
                return true;
            }
            return false;
        }

        protected object GetParam(string name, RenderParameterType type) {
            if (parameters.ContainsKey(name)) {
                var param = parameters[name];
                if (param.Type != type) {
                    throw CargoEngineException.Create("Wrong Parametertype expected " + type + " but was " + param.GetType());
                }
                return param.Value;
            }
            return null;
        }
    }

    public class ParameterManager : ParameterCollection
    {

        private static string WORLDMATRIX = "worldMatrix";
        private static string VIEWMATRIX = "viewMatrix";
        private static string PROJMATRIX = "projMatrix";
        private static string WORLDVIEWPROJMATRIX = "worldViewProjMatrix";
        private static string VIEWPROJMATRIX = "viewProjMatrix";
        private static string INV_VIEWPROJMATRIX = "invViewProjMatrix";

        public ParameterManager() {
            SetParameter(WORLDMATRIX, Matrix.Identity);
            SetParameter(VIEWMATRIX, Matrix.Identity);
            SetParameter(PROJMATRIX, Matrix.Identity);
            SetParameter(WORLDVIEWPROJMATRIX, Matrix.Identity);
        }

        public void SetWorldMatrix(Matrix world) {
            SetParameter(WORLDMATRIX, world);
            UpdateMatrices();
        }
        public void SetViewMatrix(Matrix view) {
            SetParameter(VIEWMATRIX, view);
            UpdateMatrices();
        }
        public void SetProjectionMatrix(Matrix projection) {
            SetParameter(PROJMATRIX, projection);
            UpdateMatrices();
        }

        public void ApplyCollection(ParameterCollection collection) {
            foreach (var param in collection.Parameters) {
                switch (param.Value.Type) {
                    case RenderParameterType.Vector3:
                        SetParameter(param.Key, (Vector3)param.Value.Value);
                        break;
                    case RenderParameterType.Vector4:
                        SetParameter(param.Key, (Vector4)param.Value.Value);
                        break;
                    case RenderParameterType.Matrix:
                        SetParameter(param.Key, (Matrix)param.Value.Value);
                        break;
                    case RenderParameterType.SRV:
                        SetParameter(param.Key, (ShaderResourceView)param.Value.Value);
                        break;
                    case RenderParameterType.SamplerState:
                        SetParameter(param.Key, (SamplerState)param.Value.Value);
                        break;
                    case RenderParameterType.Texture:
                        if (param.Value != null && param.Value.Value != null) {
                            SetParameter(param.Key, ((Texture.Texture)param.Value.Value).SRV);
                        } else {
                            SetParameter(param.Key, (ShaderResourceView)null);
                        }
                        break;
                }
            }
        }

        private void UpdateMatrices() {
            Matrix world = (Matrix)GetParam(WORLDMATRIX, RenderParameterType.Matrix);
            Matrix view = (Matrix)GetParam(VIEWMATRIX, RenderParameterType.Matrix);
            Matrix proj = (Matrix)GetParam(PROJMATRIX, RenderParameterType.Matrix);
            Matrix viewProj = view * proj;
            Matrix invViewProj = Matrix.Invert(viewProj);
            SetParameter(WORLDVIEWPROJMATRIX, world * view * proj);
            SetParameter(VIEWPROJMATRIX, viewProj);
            SetParameter(INV_VIEWPROJMATRIX, invViewProj);
        }
    }
}
