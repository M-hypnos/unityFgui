local main = {}

print("==========>>  main.lua")

main.init = function()
    print("==========>>  main.init")
    local test = require("test")

end

main.update = function()
    -- print("==========>>  main.update")
end

main.exit = function()
    print("==========>>  main.exit")
end

return main