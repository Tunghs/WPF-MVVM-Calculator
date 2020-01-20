using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wpf_MVVM_Calculator
{
    public class CalcViewModel : INotifyPropertyChanged
    {
        // 출력 될 문자를 담아둘 변수
        string inputString = "";
        // 계산기 화면의 출력 텍스트 박스에 대응되는 필드
        string displayText = "";
        // 속성이 바뀔 때 이벤트가 발생하다록 이벤트 정의
        public event PropertyChangedEventHandler PropertyChanged;

        // 생성자
        public CalcViewModel()
        {
            // 숫자 버튼을 클릭할 때 실행
            this.AddCharCommand = new AddCharCommand(this);
            //백스페이스 버튼을 클릭할 때 실행, 한글자 삭제
            this.DeleteCharCommand = new DeleteCharCommand(this);
            //Clear 버튼을 클릭할 때 실행, 출력창을 전부 지운다.
            this.ClearCommand = new ClearCommand(this);
            //+,-,*,/ 버튼을 클릭할 때 실행
            //현재출력창의 숫자를 Op1 속성에 저장, Op속성에 클릭한 연산자 저장
            //계산기의 출력화면을 Clear
            this.OperationCommand = new OperationCommand(this);
            // =버튼을 클릭시 실행
            this.CalcCommand = new CalcCommand(this);
        }

        public string InputString
        {
            internal set
            {
                if (inputString != value)
                {
                    inputString = value;
                    OnPropertyChanged("InputString");
                    this.DisplayText = inputString;
                    ((DeleteCharCommand)this.DeleteCharCommand).OnCanExecuteChanged();
                }
            }
            get { return inputString; }
        }

        // 계산기의 출력창과 바인딩 된 속성
        public string DisplayText
        {
            protected set
            {
                if (displayText != value)
                {
                    displayText = value;
                    OnPropertyChanged("DisplayText");
                }
            }
            get { return displayText; }
        }

        public string Op { get; set; }
        public double Op1 { get; set; }
        public double Op2 { get; set; }

        // 숫자 클릭
        public ICommand AddCharCommand { protected set; get; }
        // <- 클릭, 한글자씩 삭제
        public ICommand DeleteCharCommand { protected set; get; }
        // C 클릭시 DsplayText 전체를 삭제
        public ICommand ClearCommand { protected set; get; }
        // 사칙연산 클릭
        public ICommand OperationCommand { protected set; get; }
        // = 클릭
        public ICommand CalcCommand { protected set; get; }

        protected void OnPropertyChanged(string propertyName)
        {
            // 이벤트를 발생
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // CanExecute를 호출하기 위한 메소드
    // 예를들면 처음 화면이 뜰 때 BACK 버튼이 비활성화 되었다가
    // 입력이 들어오면 활성화 될 때 이 메소드를 호출한다.
    public interface IBaseCommand : ICommand
    {
        void OnCanExecuteChanged();
    }

    class AddCharCommand : ICommand
    {
        private CalcViewModel viewModel;
        public event EventHandler CanExecuteChanged;

        public AddCharCommand(CalcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            viewModel.InputString += parameter;
        }
    }

    class DeleteCharCommand : IBaseCommand
    {
        private CalcViewModel viewModel;

        // OnCanExecuteChanged 메소드의
        // ommandManager.InvalidateRequerySuggested()를 호출하면
        // CanExecuteChanged 이벤트가 호출되어
        // CanExecute로 해당 Command가 있는 버튼을 활성화 또는 비활성화
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DeleteCharCommand(CalcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            return viewModel.InputString.Length > 0;
        }

        public void Execute(object parameter)
        {
            viewModel.InputString = viewModel.InputString.Substring(0, viewModel.InputString.Length - 1);
        }
    }

    class ClearCommand : IBaseCommand
    {
        private CalcViewModel viewModel;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public ClearCommand(CalcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return viewModel.InputString.Length > 0;
        }

        // Clear
        public void Execute(object parameter)
        {
            viewModel.InputString = "";
        }

        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    class OperationCommand : IBaseCommand
    {
        private CalcViewModel viewModel;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public OperationCommand(CalcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            return viewModel.InputString.Length > 0;
        }

        public void Execute(object parameter)
        {
            viewModel.Op = parameter.ToString();
            viewModel.Op1 = Convert.ToDouble(viewModel.InputString);
            viewModel.InputString = "";
        }
    }

    class CalcCommand : IBaseCommand
    {
        private CalcViewModel viewModel;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested += value; }
        }

        public CalcCommand(CalcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            return (viewModel.Op1.ToString() != string.Empty && viewModel.Op2.ToString() != string.Empty);
        }

        public void Execute(object parameter)
        {
            viewModel.Op2 = Convert.ToDouble(viewModel.InputString);

            switch (viewModel.Op)
            {
                case "+": viewModel.InputString = (viewModel.Op1 + viewModel.Op2).ToString(); break;
                case "-": viewModel.InputString = (viewModel.Op1 - viewModel.Op2).ToString(); break;
                case "*": viewModel.InputString = (viewModel.Op1 * viewModel.Op2).ToString(); break;
                case "/": viewModel.InputString = (viewModel.Op1 / viewModel.Op2).ToString(); break;
            }
            viewModel.Op1 = Convert.ToDouble(viewModel.InputString);
        }
    }
}
