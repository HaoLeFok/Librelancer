require 'objects'
require 'childwindowmanager'

local function ModelRenderable(model, tint)
	local renderable = NewObject('UiRenderable')
	local modelElem = NewObject('DisplayModel')
	modelElem.Model = model
	if tint ~= nil then
		modelElem.Tint = tint
	end
	renderable:AddElement(modelElem)
	
	return renderable
end

local function NavbarButton(hotspot, active)
	-- State
	local normalTint = GetColor('white')
	if active == true then
		normalTint = GetColor('yellow')
	end
	-- Construct Appearance
	local model = NewObject('InterfaceModel')
	model.Path = GetNavbarIconPath(hotspot)
	model.X = 0
	model.Y = 0
	model.XScale = 35.0
	model.YScale = 35.0
	local button = NewObject('Button')
	local style = NewObject('ButtonStyle')
	style.Width = 38
	style.Height = 38
	local regAppearance = NewObject('ButtonAppearance')
	regAppearance.Background = ModelRenderable(model, normalTint)
	style.Normal = regAppearance
	local hoverAppearance = NewObject('ButtonAppearance')
	if active == true then
		hoverAppearance.Background = regAppearance.Background
	else
		hoverAppearance.Background = ModelRenderable(model, GetColor('white_hover'))
	end
	style.Hover = hoverAppearance
	-- Set Appearance
	button.ID = hotspot
	button:SetStyle(style)
	return button
end

local function NavbarAction(hotspot)
	local obj = NavbarButton(hotspot, false)
	obj.Width = 33
	obj.Height = 33
	return obj
end

local navbox = require 'navbox.lua'

function baseside:ctor()
   navbox = require 'navbox.lua'

    local btns = Game:GetNavbarButtons()
    local actions = Game:GetActionButtons()
    local activeids = Game:ActiveNavbarButton()
    local container = navbox.GetNavbox(self.Widget, btns)
    local locX = navbox.GetStartX(btns)
    local activeIDS = 0

    for index, button in ipairs(btns) do
        local obj = NavbarButton(button.IconName, button.IDS == activeids)
        obj.Anchor = AnchorKind.TopCenter
        obj.X = locX
        locX = locX + navbox.XSpacing
        obj.Y = navbox.OffsetY
        if button.IDS ~= activeids then
            obj:OnClick(function()
                Game:HotspotPressed(button.IDS)
            end)
        else
            activeIDS = index
        end
        container:AddChild(obj)
    end

	local has_news = false
	local news_button = {}

	local has_commodity = false
	local has_equipment = false
	local commodity_button = {}

	local has_shipdealer = false
	local shipdealer_button = {}
	
    local actionbox = navbox.GetActionBox(self.Widget, container, btns, actions, activeIDS)
    for index, action in ipairs(actions) do
        local obj = NavbarAction(action.IconName)
		if action.IconName == "IDS_HOTSPOT_NEWSVENDOR" then
			has_news = true
			news_button = obj
		elseif action.IconName == "IDS_HOTSPOT_COMMODITYTRADER" then
			has_commodity = true
			commodity_button = obj
		elseif action.IconName == "IDS_HOTSPOT_EQUIPMENTDEALER" then
			has_equipment = true
			commodity_button = obj
		elseif action.IconName == "IDS_HOTSPOT_SHIPDEALER" then
			has_shipdealer = true
			shipdealer_button = obj
		else
        	obj:OnClick(function()
            	Game:HotspotPressed(action.IDS)
        	end)
		end
        navbox.PositionAction(obj, actionbox, index)
    end
    
    self.Elements.chatbox:OnTextEntered(function (text)
                                            Game:TextEntered(text)
                                        end)

	self.InfoWindow = infowindow()
	self.Map = mapwindow()
	self.Map:InitMap()
	self.CommodityTrader = commodity()
	local windows = {
		{ self.Elements.nn_map, self.Map },
		{ self.Elements.nn_info, self.InfoWindow },
	}
	if has_news then
		self.News = news()
		table.insert(windows, { news_button, self.News })
	end
	if has_commodity then
		self.CommodityTrader = commodity("commodity")
		table.insert(windows, { commodity_button, self.CommodityTrader })
	elseif has_equipment then
		self.CommodityTrader = commodity("equipment")
		table.insert(windows, { commodity_button, self.CommodityTrader })
	elseif has_shipdealer then
		self.ShipDealer = shipdealer()
		table.insert(windows, { shipdealer_button, self.ShipDealer })
	end
	self.WindowManager = childwindowmanager(self.Widget, windows)

	self.Elements.chat.Chat = Game:GetChats()
end

function baseside:Pause()
	OpenModal(pausemenu())
end

function baseside:Chatbox()
   self.Elements.chatbox.Visible = true 
end

function baseside:Popup(title, contents, id)
	OpenModal(popup(title,contents, 'ok', function()
		Game:PopupFinish(id)
	end))
end

function baseside:MissionOffer(mission)
	OpenModal(popup(STRID_MISSION,mission, 'accept', function(result)
		Game:MissionResponse(result)
	end))
end






















