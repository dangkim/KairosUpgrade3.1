
import React, { Fragment } from 'react';
import { connect } from 'react-redux';
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

class CompareInfluencers extends React.Component {
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
        this.setState({ dropdownOpen: true });
    }

    onMouseLeave() {
        this.setState({ dropdownOpen: false });
    }

    render() {
        const { ComparedInfluencers } = this.props;
        debugger;

        const strOfReaction1 = ComparedInfluencers.length > 0 ? ComparedInfluencers[0].numberOfReaction : '';
        const strOfComment1 = ComparedInfluencers.length > 0 ? ComparedInfluencers[0].numberOfComment : '';
        const strOfShare1 = ComparedInfluencers.length > 0 ? ComparedInfluencers[0].numberOfShare : '';
        const numberOfReaction1 = strOfReaction1.charAt(strOfReaction1.length - 1) == 'k' ? Number((strOfReaction1.substring(0, strOfReaction1.length - 1))) * 1000 : Number(strOfReaction1);
        const numberOfComment1 = strOfComment1.charAt(strOfComment1.length - 1) == 'k' ? Number((strOfComment1.substring(0, strOfComment1.length - 1))) * 1000 : Number(strOfComment1);
        const numberOfShare1 = strOfShare1.charAt(strOfShare1.length - 1) == 'k' ? Number((strOfShare1.substring(0, strOfShare1.length - 1))) * 1000 : Number(strOfShare1);
        const engagement1 = numberOfReaction1 + (numberOfComment1 * 2) + (numberOfShare1 * 3)

        const strOfReaction2 = ComparedInfluencers.length > 0 ? ComparedInfluencers[1].numberOfReaction : '';
        const strOfComment2 = ComparedInfluencers.length > 0 ? ComparedInfluencers[1].numberOfComment : '';
        const strOfShare2 = ComparedInfluencers.length > 0 ? ComparedInfluencers[1].numberOfShare : '';
        const numberOfReaction2 = strOfReaction2.charAt(strOfReaction2.length - 1) == 'k' ? Number((strOfReaction2.substring(0, strOfReaction2.length - 1))) * 1000 : Number(strOfReaction2);
        const numberOfComment2 = strOfComment2.charAt(strOfComment2.length - 1) == 'k' ? Number((strOfComment2.substring(0, strOfComment2.length - 1))) * 1000 : Number(strOfComment2);
        const numberOfShare2 = strOfShare2.charAt(strOfShare2.length - 1) == 'k' ? Number((strOfShare2.substring(0, strOfShare2.length - 1))) * 1000 : Number(strOfShare2);
        const engagement2 = numberOfReaction2 + (numberOfComment2 * 2) + (numberOfShare2 * 3)

        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <Row>
                            <Col md="12">
                                <Card className="main-card mb-3">
                                    <CardBody>
                                        <CardTitle>
                                            <Row>
                                                <Col md="6">
                                                    {ComparedInfluencers.length > 0 ? ComparedInfluencers[0].fullName : ''}
                                                </Col>
                                                <Col md="6">
                                                    {ComparedInfluencers.length > 0 ? ComparedInfluencers[1].fullName : ''}
                                                </Col>
                                            </Row>

                                        </CardTitle>
                                        <Row>
                                            <Col>
                                                <Nav vertical>
                                                    <NavItem className="nav-item-header">
                                                        Interaction
                                                </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-like2"> </i>
                                                            <span>Reactions(Love|Like|Smile..)</span>
                                                            <div className="ml-auto badge badge-pill badge-warning">{numberOfReaction1}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-chat"> </i>
                                                            <span>Comments</span>
                                                            <div className="ml-auto badge badge-pill badge-info">{numberOfComment1}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-share"> </i>
                                                            <span>Share</span>
                                                            <div className="ml-auto badge badge-pill badge-alternate">{numberOfShare1}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-graph"> </i>
                                                            <span>Engagement</span>
                                                            <div className="ml-auto badge badge-pill badge-danger">{engagement1}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem className="nav-item-header">
                                                        Activity
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-users"> </i>
                                                            <span>Followers</span>
                                                            <div className="ml-auto badge badge-success">{ComparedInfluencers.length > 0 ? ComparedInfluencers[0].numberOfFollowers : ''}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-note"> </i>
                                                            <span>Posts</span>
                                                            <div className="ml-auto badge badge-secondary">{ComparedInfluencers.length > 0 ? ComparedInfluencers[0].numberOfPost : ''}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem className="nav-item-divider" />
                                                </Nav>
                                            </Col>
                                            <Col>
                                                <Nav vertical>
                                                    <NavItem className="nav-item-header">
                                                        Interaction
                                                </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-like2"> </i>
                                                            <span>Reactions(Love|Like|Smile..)</span>
                                                            <div className="ml-auto badge badge-pill badge-warning">{numberOfReaction2}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-chat"> </i>
                                                            <span>Comments</span>
                                                            <div className="ml-auto badge badge-pill badge-info">{numberOfComment2}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-share"> </i>
                                                            <span>Share</span>
                                                            <div className="ml-auto badge badge-pill badge-alternate">{numberOfShare2}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-graph"> </i>
                                                            <span>Engagement</span>
                                                            <div className="ml-auto badge badge-pill badge-danger">{engagement2}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem className="nav-item-header">
                                                        Activity
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-users"> </i>
                                                            <span>Followers</span>
                                                            <div className="ml-auto badge badge-success">{ComparedInfluencers.length > 0 ? ComparedInfluencers[1].numberOfFollowers : ''}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem>
                                                        <NavLink>
                                                            <i className="nav-link-icon pe-7s-note"> </i>
                                                            <span>Posts</span>
                                                            <div className="ml-auto badge badge-secondary">{ComparedInfluencers.length > 0 ? ComparedInfluencers[1].numberOfPost : ''}</div>
                                                        </NavLink>
                                                    </NavItem>
                                                    <NavItem className="nav-item-divider" />
                                                </Nav>
                                            </Col>
                                        </Row>
                                    </CardBody>
                                </Card>
                            </Col>
                        </Row></CSSTransition>
                </TransitionGroup>
            </Fragment>
        );
    }
};

function mapStateToProps(state) {

    const { campaigns, influencers, locations, interestings, jobCategories, jobs, brands } = state;
    //const {brand} = influencers;
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

const connectedCompareInfluencers = connect(mapStateToProps)(CompareInfluencers);
export { connectedCompareInfluencers as CompareInfluencers };