import React, {Fragment} from 'react';
import {
    CSSTransition,
    TransitionGroup,
  } from 'react-transition-group';
import {
    Button, UncontrolledButtonDropdown,
    DropdownToggle, Dropdown, DropdownMenu,
    DropdownItem,
} from 'reactstrap';

import {
    Row, Col,
    Card, CardBody,
    CardTitle, Nav, NavItem, NavLink

} from 'reactstrap';

export default class NavsVertical extends React.Component {
    constructor(props) {
        super(props);

        this.toggle = this.toggle.bind(this);
        this.onMouseEnter = this.onMouseEnter.bind(this);
        this.onMouseLeave = this.onMouseLeave.bind(this);
        this.state = {
            dropdownOpen: false
        };
    }

    toggle() {
        this.setState(prevState => ({
            dropdownOpen: !prevState.dropdownOpen
        }));
    }

    onMouseEnter() {
        this.setState({dropdownOpen: true});
    }

    onMouseLeave() {
        this.setState({dropdownOpen: false});
    }

    render() {
        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                    <Row>
                        <Col md="12">
                            <Card className="main-card mb-3">
                                <CardBody>
                                    <CardTitle>Separators & Headers</CardTitle>
                                    <Row>
                                        <Col>
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
                                                <NavItem className="nav-item-divider"/>
                                                <NavItem className="nav-item-btn">
                                                    <Button size="sm" className="btn-wide btn-shadow" color="danger">
                                                        Cancel
                                                    </Button>
                                                </NavItem>
                                            </Nav>
                                        </Col>
                                        <Col>
                                            <Nav vertical>
                                                <NavItem className="nav-item-header">
                                                    Activity
                                                </NavItem>
                                                <NavItem>
                                                    <NavLink href="javascript:void(0);">
                                                        <i className="nav-link-icon pe-7s-chat"> </i>
                                                        <span>Chat</span>
                                                        <div className="ml-auto badge badge-pill badge-info">8</div>
                                                    </NavLink>
                                                </NavItem>
                                                <NavItem>
                                                    <NavLink href="javascript:void(0);">
                                                        <i className="nav-link-icon pe-7s-wallet"> </i>
                                                        <span>Recover Password</span>
                                                    </NavLink>
                                                </NavItem>
                                                <NavItem className="nav-item-header">
                                                    My Account
                                                </NavItem>
                                                <NavItem>
                                                    <NavLink href="javascript:void(0);">
                                                        <i className="nav-link-icon pe-7s-config"> </i>
                                                        <span>Settings</span>
                                                        <div className="ml-auto badge badge-success">New</div>
                                                    </NavLink>
                                                </NavItem>
                                                <NavItem>
                                                    <NavLink href="javascript:void(0);">
                                                        <i className="nav-link-icon pe-7s-coffee"> </i>
                                                        <span>Messages</span>
                                                        <div className="ml-auto badge badge-warning">512</div>
                                                    </NavLink>
                                                </NavItem>
                                                <NavItem>
                                                    <NavLink href="javascript:void(0);">
                                                        <i className="nav-link-icon pe-7s-box2"> </i>
                                                        <span>Logs</span>
                                                    </NavLink>
                                                </NavItem>
                                                <NavItem className="nav-item-divider"/>
                                                <NavItem className="nav-item-btn">
                                                    <Button size="sm" className="btn-pill" color="success">
                                                        Save
                                                    </Button>
                                                </NavItem>
                                            </Nav>
                                        </Col>
                                    </Row>
                                    <div className="divider"/>
                                    <div className="text-center">
                                        <UncontrolledButtonDropdown direction="up">
                                            <DropdownToggle caret className="mb-2 mr-2" color="primary">
                                                Dropdown Basic
                                            </DropdownToggle>
                                            <DropdownMenu>
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
                                                    <NavItem className="nav-item-divider"/>
                                                    <NavItem className="nav-item-btn">
                                                        <Button size="sm" className="btn-wide btn-shadow"
                                                                color="danger">
                                                            Cancel
                                                        </Button>
                                                    </NavItem>
                                                </Nav>
                                            </DropdownMenu>
                                        </UncontrolledButtonDropdown>
                                        <UncontrolledButtonDropdown direction="up">
                                            <DropdownToggle caret color="primary" className="mb-2 mr-2">
                                                Dropdown Example
                                            </DropdownToggle>
                                            <DropdownMenu>
                                                <Nav vertical>
                                                    <NavItem className="nav-item-header">
                                                        Activity
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink href="javascript:void(0);">
                                                            <i className="nav-link-icon pe-7s-chat"> </i>
                                                            <span>Chat</span>
                                                            <div className="ml-auto badge badge-pill badge-info">8
                                                            </div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink href="javascript:void(0);">
                                                            <i className="nav-link-icon pe-7s-wallet"> </i>
                                                            <span>Recover Password</span>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem className="nav-item-header">
                                                        My Account
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink href="javascript:void(0);">
                                                            <i className="nav-link-icon pe-7s-config"> </i>
                                                            <span>Settings</span>
                                                            <div className="ml-auto badge badge-success">New</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink href="javascript:void(0);">
                                                            <i className="nav-link-icon pe-7s-coffee"> </i>
                                                            <span>Messages</span>
                                                            <div className="ml-auto badge badge-warning">512</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink href="javascript:void(0);">
                                                            <i className="nav-link-icon pe-7s-box2"> </i>
                                                            <span>Logs</span>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem className="nav-item-divider"/>
                                                    <NavItem className="nav-item-btn">
                                                        <Button size="sm" className="btn-pill" color="success">
                                                            Save
                                                        </Button>
                                                    </NavItem>
                                                </Nav>
                                            </DropdownMenu>
                                        </UncontrolledButtonDropdown>
                                    </div>
                                </CardBody>
                            </Card>
                        </Col>                        
                    </Row></CSSTransition>
                </TransitionGroup>
            </Fragment>
        );
    }
};
