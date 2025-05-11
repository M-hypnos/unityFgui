#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;
using Tutorial;

namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class TutorialDerivedClassWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Tutorial.DerivedClass);
			Utils.BeginObjectRegister(type, L, translator, 1, 0, 0, 0);
			Utils.RegisterFunc(L, Utils.OBJ_META_IDX, "__add", __AddMeta);
            
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Tutorial.DerivedClass does not have a constructor!");
        }
        
		
        
		
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __AddMeta(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
			
				if (translator.Assignable<Tutorial.DerivedClass>(L, 1) && translator.Assignable<Tutorial.DerivedClass>(L, 2))
				{
					Tutorial.DerivedClass leftside = (Tutorial.DerivedClass)translator.GetObject(L, 1, typeof(Tutorial.DerivedClass));
					Tutorial.DerivedClass rightside = (Tutorial.DerivedClass)translator.GetObject(L, 2, typeof(Tutorial.DerivedClass));
					
					translator.Push(L, leftside + rightside);
					
					return 1;
				}
            
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to right hand of + operator, need Tutorial.DerivedClass!");
            
        }
        
        
        
        
        
        
        
        
		
		
		
		
    }
}
