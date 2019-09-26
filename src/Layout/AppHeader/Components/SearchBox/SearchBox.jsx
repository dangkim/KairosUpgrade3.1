import React, { Fragment } from 'react';
import { connect } from 'react-redux';
import cx from 'classnames';

class SearchBox extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            activeSearch: false,
            searchValue: ''
        };
        this.handleChange = this.handleChange.bind(this);
    }

    handleChange(event) {
        const { name, value } = event.target;
        const { searchValue } = this.state;
        this.setState({
            [name]: value
        });
    }

    render() {
        const { activeSearch, searchValue } = this.state
        return (
            <Fragment>
                <div className={cx("search-wrapper", {
                    'active': activeSearch
                })}>
                    <div className="input-holder">
                        <input type="text" className="search-input" id="searchValue" name="searchValue" value={searchValue} onChange={this.handleChange} />
                        <button onClick={() => {
                            if (activeSearch === false) {
                                this.setState({ activeSearch: !activeSearch })
                            }
                            else {
                                this.props.handlerFromParent(searchValue)
                                this.setState({ activeSearch: !activeSearch })
                            }
                        }}
                            className="search-icon"><span /></button>
                    </div>
                    <button onClick={() => this.setState({ activeSearch: !activeSearch, searchValue: '' })} className="close" />
                </div>
            </Fragment>
        )
    }
}
function mapStateToProps(state) {
    //
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

const connectedSearchBox = connect(mapStateToProps)(SearchBox);
export { connectedSearchBox as SearchBox };
//export default SearchBox;