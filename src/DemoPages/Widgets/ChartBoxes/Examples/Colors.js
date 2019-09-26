import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';
import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';
import {
    Row, Col,
    Button,
    UncontrolledButtonDropdown,
    DropdownToggle,
    DropdownMenu,
    Nav,
    NavItem,
    NavLink
} from 'reactstrap';
import { infActions, brandActions } from '../../../../_actions';
import {
    AreaChart, Area, LineChart, Line,
    ResponsiveContainer,
    BarChart, Bar,
    ComposedChart,
    CartesianGrid
} from 'recharts';

import {
    faAngleUp,
    faAngleDown,
    faArrowLeft,
    faArrowRight,
    faEllipsisH,

} from '@fortawesome/free-solid-svg-icons';

import bg1 from '../../../../assets/utils/images/dropdown-header/abstract1.jpg';

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const data = [
    { name: 'Page A', uv: 4000, pv: 2400, amt: 2400 },
    { name: 'Page B', uv: 3000, pv: 1398, amt: 2210 },
    { name: 'Page C', uv: 2000, pv: 9800, amt: 2290 },
    { name: 'Page D', uv: 2780, pv: 3908, amt: 2000 },
    { name: 'Page E', uv: 1890, pv: 4800, amt: 2181 },
    { name: 'Page F', uv: 2390, pv: 3800, amt: 2500 },
    { name: 'Page G', uv: 3490, pv: 4300, amt: 2100 },
    { name: 'Page C', uv: 2000, pv: 6800, amt: 2290 },
    { name: 'Page D', uv: 4780, pv: 7908, amt: 2000 },
    { name: 'Page E', uv: 2890, pv: 9800, amt: 2181 },
    { name: 'Page F', uv: 1390, pv: 3800, amt: 1500 },
    { name: 'Page G', uv: 3490, pv: 4300, amt: 2100 },
];

const data2 = [
    { name: 'Page A', uv: 5400, pv: 5240, amt: 1240 },
    { name: 'Page B', uv: 7300, pv: 4139, amt: 3221 },
    { name: 'Page C', uv: 8200, pv: 7980, amt: 5229 },
    { name: 'Page D', uv: 6278, pv: 4390, amt: 3200 },
    { name: 'Page E', uv: 3189, pv: 7480, amt: 6218 },
    { name: 'Page D', uv: 9478, pv: 6790, amt: 2200 },
    { name: 'Page E', uv: 1289, pv: 1980, amt: 7218 },
    { name: 'Page F', uv: 3139, pv: 2380, amt: 5150 },
    { name: 'Page G', uv: 5349, pv: 3430, amt: 3210 },
];

class ColorsExample extends Component {

    componentDidMount() {
        
        const { dispatch } = this.props;
        //const { first } = this.state;
        dispatch(infActions.getAll(9, 0));
    }

    render() {
        
        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <div>
                            {
                                //(this.props.influencers && this.props.influencers.items && this.props.influencers.items.length > 0) ?
                                <Row>

                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div className="card mb-3 bg-success widget-chart text-white card-border">
                                            <div className="widget-chart-actions">
                                                <UncontrolledButtonDropdown>
                                                    <DropdownToggle color="link" className="text-white">
                                                        <FontAwesomeIcon icon={faEllipsisH} />
                                                    </DropdownToggle>
                                                    <DropdownMenu className="dropdown-menu-lg dropdown-menu-right">
                                                        <Nav vertical>
                                                            <NavItem className="nav-item-header">
                                                                Activity
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Chat
                                                            <div className="ml-auto badge badge-pill badge-info">8</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">Recover Password</NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-header">
                                                                My Account
                                                    </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Settings
                                                            <div className="ml-auto badge badge-success">New</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Messages
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                                </NavLink>
                                                            </NavItem>
                                                            <NavItem>
                                                                <NavLink href="javascript:void(0);">
                                                                    Logs
                                                        </NavLink>
                                                            </NavItem>
                                                            <NavItem className="nav-item-divider" />
                                                            <NavItem className="nav-item-btn">
                                                                <Button size="sm" className="btn-wide btn-shadow"
                                                                    color="danger">
                                                                    Cancel
                                                        </Button>
                                                            </NavItem>
                                                        </Nav>
                                                    </DropdownMenu>
                                                </UncontrolledButtonDropdown>
                                            </div>
                                            <div className="icon-wrapper rounded-circle">
                                                <div className="icon-wrapper-bg bg-white opacity-10" />
                                                <i className="lnr-screen text-success" />
                                            </div>
                                            <div className="widget-numbers">
                                                17.2k
                                    </div>
                                            <div className="widget-subheading">
                                                Profiles
                                    </div>
                                            <div className="widget-description text-white">
                                                <span className="pr-1">175.5%</span>
                                                <FontAwesomeIcon icon={faArrowLeft} />
                                            </div>
                                        </div>
                                    </Col>

                                </Row>
                                //: ''
                            }
                        </div>
                    </CSSTransition>
                </TransitionGroup>
            </Fragment>
        );
    }
}
function mapStateToProps(state) {

    const { campaigns, influencers, locations, interestings, jobCategories, jobs, brands } = state;
    //const { brand } = influencers;
    return {
        //loggingIn,
        brands,
        jobs,
        jobCategories,
        interestings,
        locations,
        campaigns,
        influencers
    };
}

const connectedColorsExample = connect(mapStateToProps)(ColorsExample);
export { connectedColorsExample as ColorsExample };
//export default BasicExample;
