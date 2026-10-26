export class GridOption {
  pageSize: number = 1;
  pageNumber: number = 10;
  sortDirection: 'asc' | 'desc' = 'asc';
  sortColumn: string = '';
  searchText: string = '';

  constructor(pageSize: number, pageNumber: number, sortDirection: 'asc' | 'desc' = 'asc', sortColumn: string = '', searchText: string = '') {
    this.pageSize = pageSize;
    this.pageNumber = pageNumber;
    this.sortDirection = sortDirection;
    this.sortColumn = sortColumn;
    this.searchText = searchText;
  }
}