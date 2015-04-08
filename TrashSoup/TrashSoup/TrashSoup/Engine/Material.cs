﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    // A container for textures and effects associated with the model
    public class Material
    {
        #region enums

        #endregion

        #region variables

        protected Texture2D diffuseMap;
        protected Texture2D normalMap;
        protected Texture2D cubeMap;
        protected Texture2D alphaMap;

        protected Vector3 specularColor;
        protected float glossiness;

        protected Vector3 reflectivityColor;
        protected float reflectivityBias;

        protected float transparency;

        protected bool perPixelLighting;

        protected Dictionary<string, EffectParameter> parameters;
        protected List<string> dirLightsNames;

        #endregion

        #region properties

        public string Name { get; set; }

        public Texture2D DiffuseMap
        {
            get
            {
                return diffuseMap;
            }
            set
            {
                diffuseMap = value;
            }
        }

        public Texture2D NormalMap
        {
            get
            {
                return normalMap;
            }
            set
            {
                normalMap = value;
            }
        }

        public Texture2D CubeMap
        {
            get
            {
                return cubeMap;
            }
            set
            {
                cubeMap = value;
            }
        }

        public Vector3 SpecularColor
        {
            get
            {
                return specularColor;
            }
            set
            {
                specularColor = value;
            }
        }

        public float Glossiness
        {
            get
            {
                return glossiness;
            }
            set
            {
                glossiness = value;
            }
        }

        public Vector3 ReflectivityColor
        {
            get
            {
                return reflectivityColor;
            }
            set
            {
                reflectivityColor = value;
            }
        }

        public float ReflectivityBias       // 0 means 0% reflection, 100% texture, 1 means otherwise
        {
            get
            {
                return reflectivityBias;
            }
            set
            {
                reflectivityBias = value;
            }
        }

        public float Transparency       // 1.0 means opaque
        {
            get
            {
                return transparency;
            }
            set
            {
                transparency = value;
            }
        }

        public bool PerPixelLighting
        {
            get
            {
                return perPixelLighting;
            }
            set
            {
                this.perPixelLighting = value;
            }
        }

        public Effect MyEffect { get; set; }

        #endregion

        #region methods

        public Material(string name, Effect effect)
        {
            this.MyEffect = effect;
            this.parameters = new Dictionary<string, EffectParameter>();

            this.dirLightsNames = new List<string>();
            this.dirLightsNames.Add("DirLight0");
            this.dirLightsNames.Add("DirLight1");
            this.dirLightsNames.Add("DirLight2");

            this.Name = name;

            AssignParamsInitialize();

            this.DiffuseMap = ResourceManager.Instance.Textures["DefaultDiffuse"];
            this.NormalMap = ResourceManager.Instance.Textures["DefaultNormal"];
            this.CubeMap = ResourceManager.Instance.Textures["DefaultCube"];
            this.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
            this.Glossiness = 50.0f;
            this.ReflectivityColor = new Vector3(1.0f, 1.0f, 1.0f);
            this.ReflectivityBias = 0.2f;
            this.Transparency = 1.0f;
            this.perPixelLighting = false;
        }

        public Material(string name, Effect effect, Texture2D diffuse) : this(name, effect)
        {
            this.DiffuseMap = diffuse;
        }

        public void UpdateEffect()
        {
            UpdateEffect(Matrix.Identity, Matrix.Identity, null, null, null, null, null, null, 0, new Vector3(0.0f, 0.0f, 0.0f));
        }

        public void UpdateEffect(Matrix world, Matrix worldViewProj, LightAmbient amb, LightDirectional[] dirs, Vector3[] pointColors,
            Vector3[] pointSpeculars, float[] pointAttenuations, Vector3[] pointPositions, uint pointCount, Vector3 eyeVector)
        {
            this.parameters["World"].SetValue(world);
            this.parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            this.parameters["WorldViewProj"].SetValue(worldViewProj);

            // updating all because fuck you
            EffectParameter param = null;
            this.parameters.TryGetValue("DiffuseMap", out param);
            if (param != null)
            {
                param.SetValue(this.diffuseMap);
            }
            else
            {
                this.parameters.TryGetValue("Texture", out param);
                if (param != null) param.SetValue(this.diffuseMap);
            }

            param = null;
            this.parameters.TryGetValue("NormalMap", out param);
            if (param != null) param.SetValue(this.normalMap);
            param = null;
            this.parameters.TryGetValue("CubeMap", out param);
            if (param != null) param.SetValue(this.cubeMap);
            param = null;
            this.parameters.TryGetValue("CubeMap", out param);
            if (param != null) param.SetValue(this.cubeMap);
            param = null;
            this.parameters.TryGetValue("SpecularColor", out param);
            if (param != null) param.SetValue(this.specularColor);
            param = null;
            this.parameters.TryGetValue("Glossiness", out param);
            if (param != null) param.SetValue(this.glossiness);
            param = null;
            this.parameters.TryGetValue("ReflectivityColor", out param);
            if (param != null) param.SetValue(this.reflectivityColor);
            param = null;
            this.parameters.TryGetValue("ReflectivityBias", out param);
            if (param != null) param.SetValue(this.reflectivityBias);
            param = null;
            this.parameters.TryGetValue("Transparency", out param);
            if (param != null) param.SetValue(this.transparency);
            if (MyEffect is BasicEffect)
            {
                (MyEffect as BasicEffect).PreferPerPixelLighting = perPixelLighting;
            }
            else if (MyEffect is SkinnedEffect)
            {
                (MyEffect as SkinnedEffect).PreferPerPixelLighting = perPixelLighting;
            }

            // lights
            if(amb != null)
            {
                param = null;
                this.parameters.TryGetValue("AmbientLightColor", out param);
                if (param != null)
                {
                    param.SetValue(amb.LightColor);
                }
                else
                {
                    param = null;
                    this.parameters.TryGetValue("EmissiveColor", out param);
                    param.SetValue(amb.LightColor);
                }
            }

            if(dirs != null)
            {
                for (int i = 0; i < ResourceManager.DIRECTIONAL_MAX_LIGHTS; ++i )
                {
                    if(dirs[i] != null)
                    {
                        param = null;
                        this.parameters.TryGetValue(this.dirLightsNames[i] + "Direction", out param);
                        if (param != null) param.SetValue(dirs[i].LightDirection);

                        param = null;
                        this.parameters.TryGetValue(this.dirLightsNames[i] + "DiffuseColor", out param);
                        if (param != null) param.SetValue(dirs[i].LightColor);

                        param = null;
                        this.parameters.TryGetValue(this.dirLightsNames[i] + "SpecularColor", out param);
                        if (param != null) param.SetValue(dirs[i].LightSpecularColor);
                    }
                }
            }

            // point lights

            if(pointColors != null)
            {
                param = null;
                this.parameters.TryGetValue("PointLightDiffuseColors", out param);
                if (param != null) param.SetValue(pointColors);
            }

            if(pointSpeculars != null)
            {
                param = null;
                this.parameters.TryGetValue("PointLightSpecularColors", out param);
                if (param != null) param.SetValue(pointSpeculars);
            }

            if (pointPositions != null)
            {
                param = null;
                this.parameters.TryGetValue("PointLightPositions", out param);
                if (param != null) param.SetValue(pointPositions);
            }

            if(pointAttenuations != null)
            {
                param = null;
                this.parameters.TryGetValue("PointLightAttenuations", out param);
                if (param != null) param.SetValue(pointAttenuations);
            }

            param = null;
            this.parameters.TryGetValue("PointLightCount", out param);
            if (param != null) param.SetValue(pointCount);

            // eyevector
            param = null;
            this.parameters.TryGetValue("EyePosition", out param);
            if (param != null) param.SetValue(eyeVector);
        }

        public void SetEffectBones(Matrix[] bones)
        {
            EffectParameter param = null;
            this.parameters.TryGetValue("Bones", out param);
            if (param != null) param.SetValue(bones);
        }

        protected void AssignParamsInitialize()
        {
            if (MyEffect == null) throw new NullReferenceException("MyEffect iz null and you tryin' to extract params from it, nigga?");

            foreach(EffectParameter p in MyEffect.Parameters)
            {
                this.parameters.Add(p.Name, p);
            }

            if(MyEffect is BasicEffect)
            {
                (MyEffect as BasicEffect).LightingEnabled = true;
                (MyEffect as BasicEffect).DirectionalLight0.Enabled = true;
                (MyEffect as BasicEffect).DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight0.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight0.Direction = new Vector3(0.0f, 1.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight1.Enabled = true;
                (MyEffect as BasicEffect).DirectionalLight1.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight1.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight1.Direction = new Vector3(0.0f, 1.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight2.Enabled = true;
                (MyEffect as BasicEffect).DirectionalLight2.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight2.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as BasicEffect).DirectionalLight2.Direction = new Vector3(0.0f, 1.0f, 0.0f);
                (MyEffect as BasicEffect).TextureEnabled = true;
            }
            else if(MyEffect is SkinnedEffect)
            {
                (MyEffect as SkinnedEffect).EnableDefaultLighting();
                (MyEffect as SkinnedEffect).DirectionalLight0.Enabled = true;
                (MyEffect as SkinnedEffect).DirectionalLight1.Enabled = true;
                (MyEffect as SkinnedEffect).DirectionalLight2.Enabled = true;
                (MyEffect as SkinnedEffect).DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight0.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight0.Direction = new Vector3(0.0f, 1.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight1.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight1.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight1.Direction = new Vector3(0.0f, 1.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight2.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight2.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                (MyEffect as SkinnedEffect).DirectionalLight2.Direction = new Vector3(0.0f, 1.0f, 0.0f);
            }
        }
        #endregion
    }
}
