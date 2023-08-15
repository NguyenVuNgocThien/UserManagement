import { Component, ElementRef, Injector, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  PagedListingComponentBase,
  PagedRequestDto
} from 'shared/paged-listing-component-base';
import {
  UserServiceProxy,
  UserDto,
  UserDtoPagedResultDto,
  ExcelInfoDto,
  FileDto
} from '@shared/service-proxies/service-proxies';
import { CreateUserDialogComponent } from './create-user/create-user-dialog.component';
import { EditUserDialogComponent } from './edit-user/edit-user-dialog.component';
import { ResetPasswordDialogComponent } from './reset-password/reset-password.component';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { AppConsts } from '@shared/AppConsts';

class PagedUsersRequestDto extends PagedRequestDto {
  keyword: string;
  isActive: boolean | null;
}

@Component({
  templateUrl: './users.component.html',
  animations: [appModuleAnimation()]
})
export class UsersComponent extends PagedListingComponentBase<UserDto> {
  users: UserDto[] = [];
  keyword = '';
  isActive: boolean | null;
  advancedFiltersVisible = false;
  uploadUrl:string;
  selectedFile: File;
  @ViewChild('fileInput') fileInput: ElementRef;

  //@ViewChild('ExcelFileUpload',{static:false}) excelFileUpload:FileUpload;

  constructor(
    injector: Injector,
    private _userService: UserServiceProxy,
    private _httpClient:HttpClient,
    private _modalService: BsModalService,
    //private _fileDownloadService:FileDownloadService
  ) {
    super(injector);
    this.uploadUrl=AppConsts.remoteServiceBaseUrl+'/api/services/app/User/GetFileImport';
  }

  createUser(): void {
    this.showCreateOrEditUserDialog();
  }

  editUser(user: UserDto): void {
    this.showCreateOrEditUserDialog(user.id);
  }

  public resetPassword(user: UserDto): void {
    this.showResetPasswordUserDialog(user.id);
  }

  clearFilters(): void {
    this.keyword = '';
    this.isActive = undefined;
    this.getDataPage(1);
  }

  protected list(
    request: PagedUsersRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.isActive = this.isActive;

    this._userService
      .getAll(
        request.keyword,
        request.isActive,
        request.skipCount,
        request.maxResultCount
      )
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: UserDtoPagedResultDto) => {
        this.users = result.items;
        this.showPaging(result, pageNumber);
      });
  }

  protected delete(user: UserDto): void {
    abp.message.confirm(
      this.l('UserDeleteWarningMessage', user.fullName),
      undefined,
      (result: boolean) => {
        if (result) {
          this._userService.delete(user.id).subscribe(() => {
            abp.notify.success(this.l('SuccessfullyDeleted'));
            this.refresh();
          });
        }
      }
    );
  }

  private showResetPasswordUserDialog(id?: number): void {
    this._modalService.show(ResetPasswordDialogComponent, {
      class: 'modal-lg',
      initialState: {
        id: id,
      },
    });
  }

  private showCreateOrEditUserDialog(id?: number): void {
    let createOrEditUserDialog: BsModalRef;
    if (!id) {
      createOrEditUserDialog = this._modalService.show(
        CreateUserDialogComponent,
        {
          class: 'modal-lg',
        }
      );
    } else {
      createOrEditUserDialog = this._modalService.show(
        EditUserDialogComponent,
        {
          class: 'modal-lg',
          initialState: {
            id: id,
          },
        }
      );
    }

    createOrEditUserDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }
  
  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
    abp.message.confirm(
      this.l('UserImportMessage'),
      undefined,
      (result: boolean) => {
        if (result) {
          this.uploadFile(this.selectedFile);
        }
      }
    );
    }
  openFileBrowser() {
      this.fileInput.nativeElement.click();
    }
  uploadFile(file:File) {
    if (!file) {
      console.error('No file selected');
      return;
    }

    const formData = new FormData();
    formData.append('file', file);

    this._httpClient.post<string>(this.uploadUrl, formData).subscribe(
      result => {
        console.log('Upload successful');
        console.log('Response:', result);
        const fileInput = document.getElementById('fileInput') as HTMLInputElement;
        fileInput.value = '';

        this.refresh();
      },
      error => {
        console.error('Upload failed:', error);
      }
    );
  }
  downloadTempFile(file:FileDto){
    const url=AppConsts.remoteServiceBaseUrl+'/File/DownloadTempFile?fileType='+file.fileType+'&fileToken='+file.fileToken+'&fileName='+file.fileName;
    console.log(AppConsts.remoteServiceBaseUrl);
    location.href=url;
  }
  exportExcel():void{
    let itemFilter=new ExcelInfoDto();
    itemFilter.storeName="dbo.USER_DT"
    itemFilter.pathName="/wwwroot/ExcelExport/ListUserInfo.xlsx"
    this._userService.exportFileExcel(itemFilter).subscribe((res)=>{
      this.downloadTempFile(res);
      this.refresh();
    })
  }
}
