import React, { useState, useEffect, memo } from 'react';
import { Table, Input, Card, Button, Spin } from 'antd';
import { connect } from 'react-redux';
import { getCompanies, getFeatureFlag } from '../../../actions';
import CompanyProfile from './CompanyProfile';
import ApprovalTemplates from './ApprovalTemplates';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheck, faEdit } from '@fortawesome/free-solid-svg-icons';
import { useTranslation } from 'react-i18next';
import { useContext } from 'react';
import { LowFrequencyContext } from '@damcontext';

function Companies(props) {
	const { t } = useTranslation();
	const [dataSource, setDataSource] = useState([]);
	const [isCompanyUpdate, setIsCompanyUpdate] = useState(false);
	const [isApprovalTemplateUpdate, setIsApprovalTemplateUpdate] = useState(false);
	const [selectedCompany, setSelectedCompany] = useState(0);
	const [search, setSearch] = useState('');
	const { companyList, setupData } = props;
	const { approvalFlag } = useContext(LowFrequencyContext);
	const [isLoading, setIsLoading] = useState(false);

	useEffect(() => {
		setupDataSource(companyList);
	}, [companyList]);

	useEffect(() => {
		if (search.trim() == '') {
			setupDataSource(companyList);
		} else {
			let source = [];

			if (companyList.length > 0) {
				companyList.map((company) => {
					source.push({
						id: company.id,
						companyName: company.companyName,
						status: company.status
					});
				});
			}
			const filteredList = filterResultsBySearchText(source, search.trim());
			setDataSource(filteredList);
		}
	}, [search]);

	function filterResultsBySearchText(dataSource, keyword) {
		var result = dataSource.filter((company) => {
			return company.companyName.toLowerCase().includes(keyword.toLowerCase());
		});
		return result;
	}

	function setupDataSource(companies) {
		setIsLoading(true);
		let source = [];

		if (companies.length > 0) {
			companies.map((company) => {
				source.push({
					id: company.id,
					companyName: company.companyName,
					status: company.status
				});
			});
			setDataSource(source);
		}
		setIsLoading(false);
	}

	const columns = [
		{
			title: 'Company Name',
			dataIndex: 'companyName',
			key: 'companyName',
			sorter: (a, b) =>
				a.companyName.localeCompare(b.companyName, undefined, {
					numeric: true,
					sensitivity: 'base'
				}),
			sortDirections: ['ascend', 'descend']
		},
		{
			title: 'Status',
			dataIndex: 'status',
			key: 'status',
			width: 10,
			filters: [
				{
					text: 'Active',
					value: true
				},
				{
					text: 'Inactive',
					value: false
				}
			],
			onFilter: (value, record) => record.status === value,

			render: (status) => <p>{status ? 'Active' : 'Inactive'}</p>
		},
		{
			align: 'right',
			render: (record) => (
				<div>
					<Button className="option-button" onClick={() => onTableDblClick(record)}>
						<FontAwesomeIcon icon={faEdit} />
						{t('Button.Edit')}
					</Button>
					{approvalFlag && (
						<Button onClick={() => onApprovalTemplatesEditClick(record)}>
							<FontAwesomeIcon icon={faCheck} />
							{t('Label.Edit Approval Templates')}
						</Button>
					)}
				</div>
			)
		}
	];

	const onTableDblClick = (record) => {
		companyList.map((company) => {
			if (company.id === record.id) {
				setIsCompanyUpdate(true);
				setSelectedCompany(record);
			}
		});
	};

	const onApprovalTemplatesEditClick = (record) => {
		companyList.map((company) => {
			if (company.id === record.id) {
				setIsApprovalTemplateUpdate(true);
				setSelectedCompany(record);
			}
		});
	};

	return isCompanyUpdate ? (
		<CompanyProfile
			company={selectedCompany}
			setupData={setupData}
			handleBackButton={(toggle) => {
				setIsCompanyUpdate(toggle);
				setupData();
			}}
		/>
	) : isApprovalTemplateUpdate ? (
		<ApprovalTemplates
			company={selectedCompany}
			handleBackButton={(toggle) => {
				setIsApprovalTemplateUpdate(toggle);
				setupData();
			}}
		/>
	) : (
		<Spin spinning={isLoading} size="large">
			<Card
				bordered
				style={{
					marginLeft: '5%',
					marginTop: 25,
					borderRadius: 8,
					marginRight: '5%'
				}}
			>
				<Input.Search
					name="search"
					id="search"
					placeholder={t('ModalDetail.Search')}
					value={search}
					onChange={(e) => setSearch(e.target.value)}
					style={{ width: '300px' }}
				/>
			</Card>
			<Table
				onRow={(record, rowIndex) => {
					return {
						onDoubleClick: (event) => {
							onTableDblClick(record);
						} // double click row
					};
				}}
				columns={columns}
				dataSource={dataSource}
				style={{ marginLeft: '5%', marginRight: '5%', marginBottom: '5%', marginTop: '3%' }}
				scroll={{ x: 400 }}
			/>
		</Spin>
	);
}

function mapStateToProps(state) {
	return {};
}

function mapDispatchToProps(dispatch) {
	return {
		getCompanies: () => dispatch(getCompanies()),
		getFeatureFlag: () => dispatch(getFeatureFlag())
	};
}
export default connect(mapStateToProps, mapDispatchToProps)(memo(Companies));
