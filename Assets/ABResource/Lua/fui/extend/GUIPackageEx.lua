local GUIPackageEx = {}

local loadedPackages = {}

function GUIPackageEx.loadPackageAsync(url, callback)
     if CS.UnityEngine.Application.isEditor then
        if loadedPackages[url] then
			loadedPackages[url].count = loadedPackages[url].count + 1
		else
			local uiPackage = UIPackage.AddPackage("Assets/ABResources/Fgui/" .. url)
			loadedPackages[url] = {package = uiPackage, count = 1}
		end

        if callback then
			callback()
		end
     else
        if loadedPackages[url] then -- 已经加载过了
			loadedPackages[url].count = loadedPackages[url].count + 1
			callback()
		else
            
        end
     end
end