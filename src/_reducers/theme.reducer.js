import sideBar6 from '../assets/utils/images/sidebar/city1.jpg';
import { themeConstants } from '../_constants';

export function theme(state = {
  backgroundColor: 'bg-royal sidebar-text-light',
  headerBackgroundColor: 'bg-strong-bliss header-text-light',
  enableMobileMenu: '',
  enableMobileMenuSmall: '',
  enableBackgroundImage: true,
  enableClosedSidebar: false,
  enableFixedHeader: true,
  enableHeaderShadow: true,
  enableSidebarShadow: true,
  enableFixedFooter: true,
  enableFixedSidebar: true,
  colorScheme: 'white',
  backgroundImage: sideBar6,
  backgroundImageOpacity: 'opacity-06',
  enablePageTitleIcon: true,
  enablePageTitleSubheading: true,
  enablePageTabsAlt: false,
}, action) {
  switch (action.type) {
    case themeConstants.SET_ENABLE_BACKGROUND_IMAGE:
      return {
        ...state,
        enableBackgroundImage: action.enableBackgroundImage
      };

    case themeConstants.SET_ENABLE_FIXED_HEADER:
      return {
        ...state,
        enableFixedHeader: action.enableFixedHeader
      };

    case themeConstants.SET_ENABLE_HEADER_SHADOW:
      return {
        ...state,
        enableHeaderShadow: action.enableHeaderShadow
      };

    case themeConstants.SET_ENABLE_SIDEBAR_SHADOW:
      return {
        ...state,
        enableSidebarShadow: action.enableSidebarShadow
      };

    case themeConstants.SET_ENABLE_PAGETITLE_ICON:
      return {
        ...state,
        enablePageTitleIcon: action.enablePageTitleIcon
      };

    case themeConstants.SET_ENABLE_PAGETITLE_SUBHEADING:
      return {
        ...state,
        enablePageTitleSubheading: action.enablePageTitleSubheading
      };

    case themeConstants.SET_ENABLE_PAGE_TABS_ALT:
      return {
        ...state,
        enablePageTabsAlt: action.enablePageTabsAlt
      };

    case themeConstants.SET_ENABLE_FIXED_SIDEBAR:
      return {
        ...state,
        enableFixedSidebar: action.enableFixedSidebar
      };

    case themeConstants.SET_ENABLE_MOBILE_MENU:
      return {
        ...state,
        enableMobileMenu: action.enableMobileMenu
      };

    case themeConstants.SET_ENABLE_MOBILE_MENU_SMALL:
      return {
        ...state,
        enableMobileMenuSmall: action.enableMobileMenuSmall
      };

    case themeConstants.SET_ENABLE_CLOSED_SIDEBAR:
      
      return {
        ...state,
        enableClosedSidebar: action.enableClosedSidebar
      };

    case themeConstants.SET_ENABLE_FIXED_FOOTER:
      return {
        ...state,
        enableFixedFooter: action.enableFixedFooter
      };

    case themeConstants.SET_BACKGROUND_COLOR:
      return {
        ...state,
        backgroundColor: action.backgroundColor
      };

    case themeConstants.SET_HEADER_BACKGROUND_COLOR:
      return {
        ...state,
        headerBackgroundColor: action.headerBackgroundColor
      };

    case themeConstants.SET_COLOR_SCHEME:
      return {
        ...state,
        colorScheme: action.colorScheme
      };

    case themeConstants.SET_BACKGROUND_IMAGE:
      return {
        ...state,
        backgroundImage: action.backgroundImage
      };

    case themeConstants.SET_BACKGROUND_IMAGE_OPACITY:
      return {
        ...state,
        backgroundImageOpacity: action.backgroundImageOpacity
      };
    default:
      return state
  }
}
