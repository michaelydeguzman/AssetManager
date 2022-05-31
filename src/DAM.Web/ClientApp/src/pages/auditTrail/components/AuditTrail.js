import React, { useState, useEffect, memo } from 'react';
import { Layout, Table, Row, Button, Pagination, Card, Input } from 'antd';
import { connect } from 'react-redux';
import { getAuditTrails } from '../actions';
import moment from 'moment';
import { useTranslation } from 'react-i18next';

function AuditTrail(props) {
	const { t } = useTranslation();
	const [loading, setLoading] = useState(false);
	const [currentPageNumber, setCurrentPageNumber] = useState(1);
	const [pageSize, setPageSize] = useState(10);
	const [isSortAscending, setIsSortAscending] = useState(false);
	const [sortColumnName, setSortColumnName] = useState('auditCreatedDate');
	const [sortOrderCreatedDate, setSortOrderCreatedDate] = useState('descend');
	const [sortOrderAssetFile, setSortOrderAssetFile] = useState(false);
	const [fileName, setFileName] = useState('');
	const [folderName, setFolderName] = useState('');
	let auditData = props.auditData && props.auditData.assetAuditTrail;
	let totalAuditRows = props.auditData ? (props.auditData.totalCount ? props.auditData.totalCount : 0) : 0;
	const columns = [
		{
			title: () => {
				return (
					<a className="table-header-sorter" onClick={() => sortColumn('auditCreatedDate')}>
						Audit Created Date
					</a>
				);
			},
			dataIndex: 'auditCreatedDate',
			width: 200,
			ellipsis: true,
			render: (text, row, index) => {
				const auditCreatedDate = new Date(text);
				return moment(auditCreatedDate).format('DD/MM/YYYY HH:mm A');
			},
			sortOrder: sortOrderCreatedDate,
			sorter: true,
			showSorterTooltip: false
		},
		{
			title: () => {
				return (
					<a className="table-header-sorter" onClick={() => sortColumn('assetFileName')}>
						Asset File Name
					</a>
				);
			},
			dataIndex: 'assetFileName',
			width: 200,
			ellipsis: true,
			sortOrder: sortOrderAssetFile,
			sorter: true,
			showSorterTooltip: false
		},
		{
			title: 'Folder Name',
			dataIndex: 'folderName',
			width: 200,
			ellipsis: true
		},
		{
			title: 'Audit Type',
			dataIndex: 'auditTypeText',
			width: 200,
			ellipsis: true
		},
		{
			title: 'Created By',
			dataIndex: 'auditCreatedByName',
			width: 200,
			ellipsis: true
		},
		{
			title: 'Previous Parameters',
			dataIndex: 'previousParameters',
			width: 200,
			ellipsis: true
		},
		{
			title: 'New Parameters',
			dataIndex: 'newParameters',
			width: 200,
			ellipsis: true
		}
	];

	useEffect(() => {
		if (sortColumnName === 'auditCreatedDate') {
			setSortOrderAssetFile(false);

			if (isSortAscending) {
				setSortOrderCreatedDate('ascend');
			} else {
				setSortOrderCreatedDate('descend');
			}
		} else if (sortColumnName === 'assetFileName') {
			setSortOrderCreatedDate(false);

			if (isSortAscending) {
				setSortOrderAssetFile('ascend');
			} else {
				setSortOrderAssetFile('descend');
			}
		}
		reloadData();
	}, [pageSize, currentPageNumber, isSortAscending]);

	async function onLoad(props) {
		let sortOrder = '2';

		if (isSortAscending) {
			sortOrder = '1';
		} else {
			sortOrder = '2';
		}

		props.loadAuditTrails(currentPageNumber, pageSize, sortOrder, sortColumnName, fileName, folderName);
	}

	async function reloadData() {
		setLoading(true);
		onLoad(props);
		setTimeout(() => setLoading(false), 1500);
	}

	function onChangePagination(pageNumber) {
		setCurrentPageNumber(pageNumber);
	}

	function onChangePaginationSize(current, pageSize) {
		setPageSize(pageSize);
	}

	function sortColumn(columnName) {
		setSortColumnName(columnName);

		if (isSortAscending) {
			setIsSortAscending(false);
		} else {
			setIsSortAscending(true);
		}
	}
	function filterData() {
		setLoading(true);
		onLoad(props);
		setTimeout(() => setLoading(false), 1500);
	}
	function handleFileChange(e) {
		setFileName(e.target.value);
	}
	function handleFolerChange(e) {
		setFolderName(e.target.value);
	}

	return (
		<>
			<Layout className="dam-layout page-layout">
				<Layout.Content className="audit-trail-container">
					<Row className="audit-header">
						<h2 style={{ marginLeft: 12 }}>{t('Slider.Audit Trail')}</h2>
					</Row>
					<Card
						bordered
						style={{
							marginLeft: 20,
							marginTop: 25,
							borderRadius: 8,
							marginRight: 20
						}}
					>
						<Input.Search
							placeholder={t('ModalDetail.Search')}
							onChange={handleFileChange}
							className="audit-trail-search"
						/>
						<Button onClick={filterData} className="audit-trail-search-button">
							by Name
						</Button>
						<Input.Search
							placeholder={t('ModalDetail.Search')}
							onChange={handleFolerChange}
							className="audit-trail-search"
						/>
						<Button onClick={filterData} className="audit-trail-search-button">
							by Folder
						</Button>
						<Button
							type="primary"
							onClick={reloadData}
							disabled={loading}
							loading={loading}
							className="audit-trail-reload"
						>
							{t('Button.Reload')}
						</Button>
					</Card>

					<Row className="audit-body">
						<div className="table-container">
							<Table
								bordered
								columns={columns}
								dataSource={totalAuditRows > 0 ? auditData : null}
								pagination={false}
								scroll={{ x: true }}
								loading={loading}
								size="small"
							/>
							<div className="pagination-container">
								<Pagination
									showSizeChanger
									current={currentPageNumber}
									onChange={onChangePagination}
									total={totalAuditRows}
									pageSize={pageSize}
									defaultCurrent={1}
									showTotal={(total, range) => `${range[0]}-${range[1]} of ${total}`}
									onShowSizeChange={onChangePaginationSize}
								/>
							</div>
						</div>
					</Row>
				</Layout.Content>
			</Layout>
		</>
	);
}

function mapStateToProps(state) {
	return {
		auditData: state.audits.auditData
	};
}

function mapDispatchToProps(dispatch) {
	return {
		loadAuditTrails: (currentPageNumber, pageSize, sortOrder, sortColumnName, fileName, folderName) =>
			dispatch(getAuditTrails(currentPageNumber, pageSize, sortOrder, sortColumnName, fileName, folderName))
	};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(AuditTrail));
